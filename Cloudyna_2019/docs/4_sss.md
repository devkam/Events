W ramach dzisiejszego odcinka postaramy się odebrać dane, wysłane na Event Hub z wykorzystaniem komponentu Spark Streaming. Spark Streaming dostępny jest w ramach platformy Apache Spark, z którego można skorzystać za pośrednictwem usługi Azure Databricks.

### Azure Databricks

Azure Databricks jest w pełni skalowalną usługą, pozwalającą na szybkie uruchomienie klastra obliczeniowego za pomocą kilku kliknięć. Wraz z klastrem udostępnione zostaje w pełni zoptymalizowane środowisko uruchomieniowe, dostępne wewnątrz platformy Apache Spark. W ramach niego udostępniono również interaktywny edytor z wbudowaną obsługą wielu języków programowania. Usługa pozwala na przeprowadzenie wszelkiego rodzaju analiz za pomocą języków: Python, Scala, SQL czy R, oraz wykorzystanie najbardziej znanych bibliotek, wykorzystywanych w dziedzinie uczenia maszynowego, takich jak np. TensorFlow. Klaster, zaraz po stworzeniu, jest dostępny w kilka minut. Usługa ta jest w pełni skalowalna oraz doskonale integruje się z innymi usługami PasS'owymi dostępnymi w ramach chmury. Więcej o usłudze Azure Databricks możesz przeczytać w dokumentacji, dostępnej pod następującym [adresem](https://docs.microsoft.com/pl-pl/azure/azure-databricks/what-is-azure-databricks).

### Apache Spark oraz Spark Streaming

Apache Spark to potężna platforma obliczeniowa zaprojektowana na potrzeby przetwarzania ogromnych zbiorów danych Big Data. Cechą charakterystyczną Sparka jest to, że większość obliczeń realizowana jest w pamięci operacyjnej samego klastra. Pozwala to na uzyskanie bardzo wysokiej wydajności, co w przypadku problemów natury Big Data ma bardzo duże znaczenie. Dzięki wbudowanym w Sparka komponentom, takimi jak np.: **SparkML** lub **Spark Streaming** bardzo dobrze odnajduje się on również w dziedzinie uczenia maszynowego oraz przetwarzania strumieniowego. Więcej informacji na temat samego Sparka, oraz komponentu Spark Streaming znajdziesz odwiedzając następujące linki:

- [Apache Spark](https://spark.apache.org/)
- [Apache Spark Streaming](https://spark.apache.org/streaming/)


#### Praca ze strumieniami 

1. ReadStream - możemy czytać z różnych źródeł takich jak:
   1. kolejki np: Kafka
   2. Pliki np: partycojonowany ADLS 

Podobnie jak w przypadku innych operacji wewnątrz Spark warto pamiętać, że podłączenie się do strumienia nie jest operacją odczytu, a tylko deklaracją przyłącza. W wyniku, którego dostajemy tylko i wyłącznie informacje o tym co i skąd będziemy brać:

Przedstawie dwa sposoby podłączenia:

```python
df = spark.readStream \
    .format("kafka") \
    .option("subscribe", EH_TOPIC) \
    .option("kafka.bootstrap.servers", EH_NAMESPACE) \
    .option("kafka.sasl.mechanism", "PLAIN") \
    .option("kafka.security.protocol", "SASL_SSL") \
    .option("kafka.sasl.jaas.config", EH_SASL) \
    .option("failOnDataLoss", "false") \
    .load()
```

```python
df = spark \
  .readStream \
  .format("kafka") \
  .option("kafka.bootstrap.servers", "host1:port1,host2:port2") \
  .option("subscribe", "topic1") \
  .load()
```

```python
df = (spark
    .readStream                       
    .schema(jsonSchema)               
    .option("maxFilesPerTrigger", 1)  
    .json(inputPath)
    )
```
W pierwszym wypadku parametryzujemy przyłącze wykorzystując protokół kafki (0.10 lub wyższy). Definiuje także inne przydatne parametry jak `topic`, `namespace`. 

W drugim przypadku definiujemy odczyt plików ze ścieżki wejściowej `inputPath`. Dodatkowo definiujemy schemę pliku `jsonSchema` oraz porcjujemy odczyt w strumieniu by każdy plik był odczytywany osobno.

Warto wspomnieć, że w przypadku 2 to po stronie mechanizmów Spark jest stworzenie i utrzymywanie strumienia danych.

#### Schema

Spark Structure Streaming przetwarza struminie o z góry ustalonej schemie (w sposób jawny przez nas) lub przez mechanizm infereacji typów. W drugim przypadku istnieje duże prawdopodobieństwo, że mechanizm domyśli się, że pole to string :) 

Jeśli wiemy jaki typ będziemy przetwarzać warto schemę zdefiniować za wczasu.

```python
from pyspark.sql.types import StructType, StructField, StringType
userSchema = StructType().add("VendorID", "integer")
```

`StructType` to obiekt reprezentujący opis pól. Zawiera 3 użyteczne informacje. 
1. nazwę pola
2. typ pola
3. czy jest required. Domyślnie wartość to nie.

Jakie typy mamy do dyspozycji. 
1. string
2. integer
3. double
4. timestamp
5. ...

#### Transformacje i akcje

Co można robić ze strumieniem? Można go przekształcać: 
* filtrować, grupować, operacje projekcji, których wynikiem jest kolejny dataFrame. Warto zapamiętać operacje przekształcają DF w DF nie zmieniając jego stanu.
* collectować, wyświetlać, agregować. To są akcje ich wynikiem jest już materializacja wykonania. W przypadku strumieni jeszcze nie dojdzie do wykonania operacji.

Możemy operacować na strumieniu w dwojaki sposób:

1. Wywołując metody
2. Spark SQL

Warto wspomnieć, że oba podejścia dają ten sam wynik, wykorzystują te same operacje logiczne i wyniku te same operacje fizyczne.

Różnica w tym jak się piszę przeszktałcenia. Innymi słowy python/scala vs SQL.
```python
df.select("device").where("signal > 10")
df.groupBy("deviceType").count()
```
```python
df.createOrReplaceTempView("updates")
spark.sql("select count(*) from updates")
```

Idea jest taka by operacje na strumieniach i operacje na df pochodzący z plików stałych były wymienne. 

#### Wykonanie operacji na strumieniu

By wykonać operajce warto skorzystać z własności writeStream. Jest ona dostępna tylko dla dataframe, które pochodzą bezpośrednio od strumienia (spark.readStream) lub dataframe będących pochodnymi przetwarzania strumieni. 

Ponższa operacja wywoła przetworzenie strumienia wejściowego w trybie `append`. Format definiuje gdzie to będzie wyrzucone `console`. A `start` uruchomi przetwarzanie. 

``` python
df_write = df.writeStream \
    .outputMode("append") \
    .format("console") \
    .queryName("preview") \
    .start()
```

Jakie mamy inne outputMode'y:
* append - przy każdym triggerze przetwarzamy tylko nowo dochodzące wiersze
* update - przetwarzamy tylko te które się zmieniły
* complete - przy każdym triggrze przetwarzamy wszystkie wiersze, które się pojawiły.


Format definiuje nam, gdzie umieścimy wyniki przetwarzania:
* console - na konsolę - tylko dev, nigdy produkcja,
* memory - w pamięci - tylko dev, nigdy produkcja :)
* parquet - do plików parquet wraz z partycjonowaniem jeśli istnieje
* kafka - do systemu kolejkowego, który obsługuje protokół Kafki


#### Partycjonowanie pod oknie

Bardzo ciekawy mechanizm to wykorzystanie okien przesuwnych, kroczących. 

W operacjach groupBy musimy zdefiniować przynjamniej kolumne po które realizujemy grupowanie.

Warto czasami ogranicznyć przetwarzanie do pewnych okien czasowych. Do tego służy funkcja `window`, która przyjmuje kolumnę czasową jako 1 parametr, długość okna jako 2 i co ile się będzie przesuwać okno jako 3. 

Przykład. Załóżmy, że zaczynamy przetwarzanie o godzinie 10:10. ustawiamy długość okno na 10 minut i przesuwność na 5 minut.

Pierwsze okno a co za tym idzie trigger przetwarzania będzie o godzinie 10:10-10:20.
Następny o godzinie 10:15-10:25. 
I tak dalej.

```python
windowedCounts = words.groupBy(
    window(words.timestamp, "10 minutes", "5 minutes"),
    words.word
).count()
```