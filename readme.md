# Project README

## Overview

This project consists of multiple components for AI-based cryptocurrency prediction and analysis. The project is organized into two main directories: `AI` and `BE`.

## Features

- **FastAPI-powered**: Efficient backend framework for high-performance APIs.
- **AI Model Integration**: Easily deploy AI models and expose endpoints.
- **Scalable and Extensible**: Built to handle large volumes of data and user requests.
- **Asynchronous API**: Uses FastAPI’s asynchronous features for fast processing.


## Requirements

- **Python**: 3.11 or higher
- **FastAPI**: For building the API
- **Uvicorn**: ASGI server for running the FastAPI app
- **Other dependencies**: Listed in `requirements.txt`

### Directory Structure
```
AI/
├── AI_chatbot_model/
│   ├── bot/
│   ├── predict_cryptos/
│   └── rasa_bot/
├── AI_prediction_model/
│   ├── predict.py
│   ├── run_predict.py
│   ├── run.py
│   └── train.py
├── connect_db/
│   ├── config_db.py
│   └── connectDB.py
│   crawl_data/
│   ├── crawl_data_xrp.py
└── python_comute/
    └── test_crawl_stock_price.py

BE/
└── app/
    ├── routers/
    │   └── ai_analysis.py
    └── schemas/
        └── ai_analysis.py
```

## Installation

1. Clone the repository:
    ```sh
    git clone <repository_url>
    cd <repository_directory>
    ```

2. Install the required Python packages:
    ```sh
    pip install -r requirements.txt
    ```

3. Set up the database configuration in `AI/connect_db/config_db.py`.

## Usage

### Running the Prediction Model

1. Train the model:
    ```sh
    python AI/AI_prediction_model/train.py
    ```

2. Run predictions:
    ```sh
    python AI/AI_prediction_model/run_predict.py
    ```

### Running the Chatbot

1. Start the Rasa server:
    ```sh
    rasa run
    ```

2. Start the action server:
    ```sh
    rasa run actions
    ```

### Running the API

1. Start the FastAPI server:
    ```sh
    uvicorn BE/app/main:app --reload
    ```

## Contributing

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Create a new Pull Request.

## License

This project is licensed under the MIT License.

## Contact

For any questions or issues, please contact [locduc1999@gmail.com](mailto:locduc1999@gmail.com).
