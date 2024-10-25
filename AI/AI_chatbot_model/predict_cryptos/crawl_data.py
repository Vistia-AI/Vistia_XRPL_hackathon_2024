from binance.client import Client
import numpy as np
from decimal import *
import time
from datetime import datetime, timedelta
from config_db import config_db

craw_startTime = int((datetime.today() - timedelta(days=180)).timestamp() * 1000)

try:
    import mysql.connector as mysql
except:
    import MySQLdb as mysql


class crawlerDataBinance(object):
    COIN_INFO_IDCOIN = 0
    COIN_INFO_SYMBOL = 1
    COIN_INFO_MINQTY = 2
    COIN_INFO_TICKSIZE = 3
    COIN_INFO_STATUS = 4
    COIN_INFO_BASEASSET = 5
    COIN_INFO_QUOTEASSET = 6
    KLINE_LIMIT = 1000

    client = Client("api_key", "api_secret")

    def get_coin_info_from_binance(self):
        market = []
        symbols = self.client.get_exchange_info()['symbols']
        for symbol in symbols:
            temp = [symbol["symbol"], symbol["filters"][1]["minQty"], symbol["filters"][0]["tickSize"],
                    symbol["status"], symbol["baseAsset"], symbol["quoteAsset"]]
            market.append(temp)
        return market

    def insert_coin_info_to_db(self):
        cnx = config_db()
        cursor = cnx.cursor()
        coin_info = self.get_coin_info_from_binance()
        try:
            query_string = "INSERT INTO coin_info(symbol, minQty, tickSize, status, baseAsset, quoteAsset) VALUES (%s,%s,%s,%s,%s,%s)"
            cursor.executemany(query_string, coin_info)
            cnx.commit()
        except mysql.Error as err:
            cnx.rollback()
            print("Something went wrong: {}".format(err))
        cursor.close()
        cnx.close()
        del coin_info

    def get_coinInfo_from_db(self):
        cnx = config_db()
        cursor = cnx.cursor()
        query_string = "SELECT id, symbol, minQty, tickSize FROM coin_info"
        cursor.execute(query_string)
        coins_info = cursor.fetchall()
        cursor.close()
        cnx.close()
        return coins_info

    def insert_candlestick_data_db(self, klines, idCoin):
        cnx = config_db()
        cursor = cnx.cursor()
        klines = np.insert(klines, [0], [idCoin], axis=1).tolist()
        try:
            query_string = "INSERT INTO candlestick_data(idCoin, openTime, `open`, high, low, `close`, volume, closeTime, quoteAssetVolume, numberOfTrader, takerBuyBaseAssetVolume, takerBuyQuoteAssetVolume, `ignore`) \
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)"
            cursor.executemany(query_string, klines)
            cnx.commit()
        except mysql.Error as err:
            cnx.rollback()
            print("Something went wrong: {}".format(err))
            print(klines)
        cursor.close()
        cnx.close()
        del klines

    def get_max_closeTime_from_db(self, idCoin):
        cnx = config_db()
        cursor = cnx.cursor()
        query_string = "SELECT MAX(closeTime) FROM candlestick_data WHERE idCoin = %s" % idCoin
        cursor.execute(query_string)
        coins_info = cursor.fetchall()
        cursor.close()
        cnx.close()
        if coins_info[0][0] == None:
            return 0
        return coins_info[0][0]

    def get_klines_startTime(self, symbol, startTime=craw_startTime):
        # 30 gio row moi row cach nhau 1 gio
        return self.client.get_klines(symbol=symbol,
                                      interval=self.client.KLINE_INTERVAL_1HOUR,
                                      startTime=startTime,
                                      limit=self.KLINE_LIMIT)

    def insert_symbols_candlestick_data(self, syms=None):
        symbols = self.get_coinInfo_from_db()

        for symbol in  ["ORAI"]: # symbols:
            if symbol[self.COIN_INFO_SYMBOL] not in syms:
                continue

            closeTime = self.get_max_closeTime_from_db(symbol[self.COIN_INFO_IDCOIN])
            closeTime = max(closeTime, craw_startTime)
            klines = self.get_klines_startTime(symbol[self.COIN_INFO_SYMBOL], closeTime + 1)
            while len(klines) > 0:
                print(symbol, closeTime, datetime.fromtimestamp(closeTime / 1000),
                      datetime.fromtimestamp(klines[0][0] / 1000), len(klines))
                self.insert_candlestick_data_db(klines, symbol[self.COIN_INFO_IDCOIN])
                # print("-1", klines[0])
                closeTime = self.get_max_closeTime_from_db(symbol[self.COIN_INFO_IDCOIN])
                # print("-2")
                klines = self.get_klines_startTime(symbol[self.COIN_INFO_SYMBOL], closeTime + 1)
            del klines


if __name__ == '__main__':
    time.strftime('%X %x')
    start_time = time.time()
    crawler = crawlerDataBinance()
    # crawler.insert_coin_info_to_db() # run first time
    crawler.insert_symbols_candlestick_data()
    print("Total time get data: %f" % (time.time() - start_time))

