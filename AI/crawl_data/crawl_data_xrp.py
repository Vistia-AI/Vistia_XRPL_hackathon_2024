import os
os.environ['TZ'] = 'UTC'

from io import BytesIO 
import sys, pycurl, json
import pandas as pd, yaml
from config.binance import ls_symbols_xrp
from config.db import XRP_DB as DB
from binance.client import Client
from datetime import datetime, timedelta
from time import sleep
from sqlalchemy import create_engine, text
from sqlalchemy.ext.compiler import compiles
from sqlalchemy.sql.expression import Insert
@compiles(Insert)
def _prefix_insert_with_ignore(insert, compiler, **kw):
    return compiler.visit_insert(insert.prefix_with('IGNORE'), **kw)

client = Client()
engine = create_engine(f"mysql+pymysql://{DB['user']}:{DB['password']}@{DB['host']}:{DB['port']}/{DB['database']}")

SCHEMA = '' if DB['database'] is None or DB['database'] =='' else DB['database'] + '.'
RAW_PRICE_1 = 'xrp_orderbook_price' 
PRICE_1 = 'coin_prices' 
PRICE_1_tmp = 'coin_prices_tmp' 
SIGNAL_1 = 'f_coin_signal_1h'
SIGNAL_4 = 'f_coin_signal_4h'
SIGNAL_24 = 'f_coin_signal_1d'

sql_create_signal = """"""

sql_table_wrap = """ with a as(
select *
from(
	SELECT
		symbol
	    , open_time
	    , close_time
		, `open`
	    , ROW_NUMBER() over (PARTITION by symbol, close_time order by open_time asc) as r
	from (
		SELECT 
			 symbol
		    , open_time
            , (open_time div ({hour}*3600) + 1) * ({hour}*3600) as close_time
		    , `open`
		from {from_table}
		where open_time >= {start_time} and open_time < {end_time} {additional_conditions}
	) o
) o
WHERE r = 1
),
b as(
select *
from(
SELECT symbol
	, open_time
	, close_time
	, max(high) over (PARTITION by symbol, close_time order by open_time desc ROWS BETWEEN CURRENT ROW AND 3 FOLLOWING) as high
	, min(low) over (PARTITION by symbol, close_time order by open_time desc ROWS BETWEEN CURRENT ROW AND 3 FOLLOWING) as low
    , `close`
	, sum(volume) over (PARTITION by symbol, close_time order by open_time desc ROWS BETWEEN CURRENT ROW AND 3 FOLLOWING) as volume
	, sum(quote_asset) over (PARTITION by symbol, close_time order by open_time desc ROWS BETWEEN CURRENT ROW AND 3 FOLLOWING) as quote_asset
	, sum(num_trades) over (PARTITION by symbol, close_time order by open_time desc ROWS BETWEEN CURRENT ROW AND 3 FOLLOWING) as num_trades
	, sum(buy_base) over (PARTITION by symbol, close_time order by open_time desc ROWS BETWEEN CURRENT ROW AND 3 FOLLOWING) as buy_base
	, sum(buy_quote) over (PARTITION by symbol, close_time order by open_time desc ROWS BETWEEN CURRENT ROW AND 3 FOLLOWING) as buy_quote
    , ROW_NUMBER() over (PARTITION by symbol, close_time order by open_time desc) as r
	from (
		select
			 symbol
		    , open_time
            , (open_time div ({hour}*3600) + 1) * ({hour}*3600) as close_time
		    , high 
		    , low 
			,`close`
		    , volume 
		    , quote_asset 
		    , num_trades
		    ,buy_base 
		    ,buy_quote 
		from {from_table}
		where open_time >= {start_time} and open_time < {end_time} {additional_conditions}
	) c
) c
where r=1
),
{to_table} as(
SELECT a.symbol
	, a.open_time
	, b.close_time
	, a.`open`
	, b.high
	, b.low 
	, b.`close`
    , b.volume 
    , b.quote_asset 
    , b.num_trades
    , b.buy_base 
    , b.buy_quote 
from a inner join b on a.symbol=b.symbol and a.close_time=b.close_time
) """

sql_signal_0 = """"""
sql_insert_signal = """"""
sql_get_price = """"""
def get_data_binance(symbol, start_time, end_time):
    klines = []
    kl_g = client.get_historical_klines_generator(symbol, Client.KLINE_INTERVAL_1HOUR, start_time, end_time)
    for kline in kl_g:
        klines.append(kline)

    coin_df = pd.DataFrame(
        columns=["open_time", "open", "high", "low", "close", "volume", "close_time", "quote_asset", "num_trades", "buy_base", "buy_quote", "ignore"], 
        data=klines
    )

    coin_df.drop(columns=["close_time", "ignore"], inplace=True)

    coin_df = coin_df.apply(pd.to_numeric, errors='coerce')
    coin_df["symbol"] = symbol
    coin_df.open_time = coin_df.open_time // 1000  #  convert ms to sec unix timestamp

    return coin_df.fillna(0)

def write_data_db(db_name="coin_prices", start_time:str = "2 hour ago", end_time:str = "1 hour ago", symbols:list = ls_symbols_xrp, row_limit:int=500):
    data = pd.DataFrame(columns=["open_time", "open", "high", "low", "close", "volume", "quote_asset", "num_trades", "buy_base", "buy_quote", "symbol"])
    for symbol in symbols:
        try:
            print(f"Writing {symbol} to db...")
            data = pd.concat([data, get_data_binance(symbol, start_time, end_time)], ignore_index=True)
        except Exception as e:
            print(e)
            continue
        if len(data) >= row_limit:
            data.to_sql(name=db_name, index=False, con=engine, if_exists="append")
            data = pd.DataFrame(columns=["open_time", "open", "high", "low", "close", "volume", "quote_asset", "num_trades", "buy_base", "buy_quote", "symbol"])
    if len(data) >= 0:
        data.to_sql(name=db_name, index=False, con=engine, if_exists="append")
    print("Done!!!")

def init_signal(end_time, time, time_27, hour, from_table=PRICE_1, to_table=SIGNAL_1, cond=""):
    print(f"init_signal {hour}H: at {time} to {to_table}")
    table_name = "tmp_table"
    source_table = sql_table_wrap.format(
        from_table=from_table,
        to_table=table_name,
        hour=hour,
        start_time=time_27,
        end_time=end_time,
        additional_conditions=cond
        )
    try:
        with engine.begin() as conn:
            conn.execute(text(sql_create_signal.format(db_name=to_table)))
            sql = sql_signal_0.format(
                pre_sql=source_table,
                from_table=table_name,
                to_table=to_table,
                time_27=time_27,
                time=end_time,
                additional_conditions=cond
                )
            conn.execute(text(sql))
            # conn.commit()  # commit the transaction
    except Exception as e:
        print(e)
        print("init_signal error")

def insert_signal(end_time, time, time_1, time_27, hour, from_table=PRICE_1, to_table=SIGNAL_1, cond="", check_init=False):
    print(f"insert_signal {hour}H: at {time} to {to_table}")
    table_name = "tmp_table"
    insert_cond = "" 
    if check_init:
        # list symbol from root table
        check1 = set(pd.read_sql(f"SELECT symbol FROM {from_table} where open_time = {time} {cond}",engine)["symbol"].values)
        # list symbol from f table - also check adx, psar, rsi14 not null
        check2 = set(pd.read_sql(f"""
            SELECT symbol 
            FROM {to_table} 
            where open_time = {time_1}
                and rsi7 is not NULL 
                and rsi14 is not NULL 
                and adx is not NULL 
                and psar is not NULL 
                {cond}
        """,engine)["symbol"].values)
        init_list = list(check1 - check2)
        if len(init_list) > 0:
            init_cond = f"""and symbol in ('{"', '".join(init_list)}')"""
            # init first row for symbols if it on first run
            init_signal(end_time, time, time_27, hour, from_table=from_table, to_table=to_table,cond=init_cond)
            insert_cond = f"""and symbol in ('{"', '".join(check1 & check2)}')""" 
    try:
        with engine.begin() as conn:
            source_table = sql_table_wrap.format(from_table=from_table,to_table=table_name, hour=hour, start_time=time_27, end_time=end_time, additional_conditions=insert_cond)
            sql = sql_insert_signal.format(pre_sql=source_table, from_table=table_name, to_table=to_table, time_27=time_27, time_t=time, time_1=time_1, additional_conditions=insert_cond)
            conn.execute(text(sql))
            # conn.commit()  # commit the transaction
    except Exception as e:
        print(e)
    
def calculate_time(end_time: int=None, min=0, hour=1):
    if end_time is None:
        end_time = int(datetime.now().timestamp())

    td = min * 60 + hour * 3600
    time = end_time-td
    time_1 = end_time - 2*td
    time_27 = end_time - 28*td
    return time, time_1, time_27
 
def get_signal(end_time: int=None, cond="", check_init=False, intervals:list=['1h', '4h', '1d']):
    if end_time is None:
        end_time = int(datetime.now().timestamp())
    t_1h = 3600
    end_time = end_time // t_1h * t_1h


    if '1h' in intervals:
        time, time_1, time_27 = calculate_time(end_time,hour=1)
        insert_signal(end_time, time, time_1, time_27, hour=1, from_table=SCHEMA+PRICE_1, to_table=SCHEMA+SIGNAL_1, cond=cond, check_init=check_init)
        # print(end_time, time, time_1, time_27) 
    if '4h' in intervals and end_time % (4*t_1h) == 0:  
        time, time_1, time_27 = calculate_time(end_time,hour=4)
        insert_signal(end_time, time, time_1, time_27, hour=4, from_table=SCHEMA+PRICE_1, to_table=SCHEMA+SIGNAL_4, cond=cond, check_init=check_init)
        # print(end_time, time, time_1, time_27) 
    if '1d' in intervals and end_time % (24*t_1h) == 0:
        time, time_1, time_27 = calculate_time(end_time,hour=24)
        insert_signal(end_time, time, time_1, time_27, hour=24, from_table=SCHEMA+PRICE_1, to_table=SCHEMA+SIGNAL_24, cond=cond, check_init=check_init)
        # print(end_time, time, time_1, time_27) 

def req_post(url, data): 
    buffer = BytesIO() 
    c = pycurl.Curl() 
    c.setopt(c.URL, url) 
    c.setopt(c.POSTFIELDS, data) 
    c.setopt(c.WRITEDATA, buffer) 
    c.perform() 
    c.close() 
    response = buffer.getvalue()
    return response.decode("utf-8") 
 
def get_price_orderbook(req_break=0.5):
    url = "https://xrplcluster.com" 
    headers = { 
        'Content-Type': 'application/json', 
        'Accept-Encoding': 'gzip' 
            } 
    payload = { 
            "method": "book_offers", 
            "params": [ 
                { 
                    "taker_gets": {}, 
                    "taker_pays": {}, 
                    "limit": 1 
                } 
            ] 
        } 
    taker_gets = { 
        "currency": "", 
        "issuer": "" 
        } 
    taker_pays = { 
        "currency": "XRP" 
        } 
    
    issuers = pd.read_sql("SELECT issuer_address, token, code from xrp.xrp_issuer ", engine) 
    
    price_data = [] 
    for i, issuer in issuers.iterrows(): 
        taker_gets = { 
            "currency": issuer['code'], 
            "issuer": issuer['issuer_address'] 
        }
        payload['params'][0]['taker_gets'] = taker_gets 
        payload['params'][0]['taker_pays'] = taker_pays

        try: 
            order_book_1 = json.loads(req_post(url, json.dumps(payload))) 
        except Exception as e:
            print("ERROR: buy order ", issuer['token'])
            # print(e, payload) 
            continue 
        payload['params'][0]['taker_gets'] = taker_pays 
        payload['params'][0]['taker_pays'] = taker_gets
        try: 
            order_book_2 = json.loads(req_post(url, json.dumps(payload))) 
        except Exception as e: 
            print("ERROR: sell order ", issuer['token'])
            # print(e, payload) 
            continue 

        try: 
            buy_price = float(order_book_1["result"]["offers"][0]['quality'])
            sell_gets = float(order_book_2["result"]["offers"][0]['TakerGets']) 
            sell_pays = float(order_book_2["result"]["offers"][0]['TakerPays']['value']) 
            sell_price = sell_gets/sell_pays
            price = (buy_price + sell_price)/2 
            price_data.append([issuer['token'], price, buy_price, sell_price, buy_price - sell_price]) 
        except Exception as e:
            print("ERROR: calcualte price ", issuer['token'])
            # print(e, payload) 
            continue    
    df = pd.DataFrame(price_data, columns=['token', 'price','buy_price', 'sell_price', 'price_range']) 
    df['time'] = int(datetime.now().timestamp())  
    df.to_sql('xrp_orderbook_price', engine, if_exists='append', index=False)

def get_price_data(from_table:str=RAW_PRICE_1, to_table:str=PRICE_1, start_time:int=None, end_time:int=None):
    to_table = PRICE_1_tmp
    t_1h = 3600
    if end_time is None:
        end_time = int(datetime.now().replace(minute=0,second=0,microsecond=0).timestamp())
    else:
        end_time = end_time // t_1h * t_1h
    if start_time is None: 
        start_time = end_time - t_1h
    else:
        start_time = start_time // t_1h * t_1h
    
    print(sql_get_price.format(
        from_table=SCHEMA+from_table,
        to_table=SCHEMA+to_table,
        start_time=start_time,
        end_time=end_time
    ))

    

def main():
    end_time = int(datetime.now().timestamp())
    write_data_db()
    get_signal(end_time)


if __name__ == "__main__":
    try:
        globals()[sys.argv[1]]()
    except Exception as e:
        # run main on default
        print(e)
        main()
        # print(False)
