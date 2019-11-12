## 1. Opis danych

Zbiór dostępny jest pod następującym [adresem](https://www1.nyc.gov/site/tlc/about/tlc-trip-record-data.page).

| Column Name        | Description	Type           | 
| ------------- |:-------------:| 
|VendorID|Number|
|tpep_pickup_datetime|	Date & Time|
|tpep_dropoff_datetime|	Date & Time|
|passenger_count|	Number|
|trip_distance|	Number|
|RatecodeID	Number|
|store_and_fwd_flag|	Plain Text
|PULocationID|	Number|
|DOLocationID|	Number|
|payment_type|	Number|
|fare_amount|	Number|
|extra|	Number|
|mta_tax|	Number|
|tip_amount|	Number|
|tolls_amount|	Number|
|improvement_surcharge|	Number|
|total_amount|	Number|

Opis poszczególnych kolumn można znaleźć [tutaj](https://www1.nyc.gov/assets/tlc/downloads/pdf/data_dictionary_trip_records_yellow.pdf).

Przykładowe wiersze:

| VendorID | tpep_pickup_datetime | tpep_dropoff_datetime | passenger_count | trip_distance |RatecodeID |store_and_fwd_flag | PULocationID | DOLocationID | payment_type | fare_amount |extra |mta_tax  |extra |tolls_amount |improvement_surcharge | total_amount |
| ------------- |:-------------:| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
|2|2084 Nov 04 12:32:24 PM|2084 Nov 04 12:47:41 PM|1|1.34|1|N|238|236|2|10|0|0.5|0|0|0.3|10.8|
|2|2084 Nov 04 12:32:24 PM|2084 Nov 04 12:47:41 PM|1|1.34|1|N|238|236|2|10|0|0.5|0|0|0.3|10.8|
|2|2084 Nov 04 12:25:53 PM|2084 Nov 04 12:29:00 PM|1|0.32|1|N|238|238|2|4|0|0.5|0|0|0.3|4.8|
|2|2084 Nov 04 12:25:53 PM|2084 Nov 04 12:29:00 PM|1|0.32|1|N|238|238|2|4|0|0.5|0|0|0.3|4.8|
|2|2084 Nov 04 12:08:33 PM|2084 Nov 04 12:22:24 PM|1|1.85|1|N|236|238|2|10|0|0.5|0|0|0.3|10.8|d