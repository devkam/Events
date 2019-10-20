# Databricks notebook source
EH_TOPIC = "[EVENT_HUB_NAME]"
EH_NAMESPACE = "[EH_NAMESPACE].servicebus.windows.net:9093"
EH_SASL = 'kafkashaded.org.apache.kafka.common.security.plain.PlainLoginModule required username="$ConnectionString" password="Endpoint=sb://[EH_NAMESPACE].servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[SAS_KEY]";'

# COMMAND ----------

df = spark.readStream \
    .format("kafka") \
    .option("subscribe", EH_TOPIC) \
    .option("kafka.bootstrap.servers", EH_NAMESPACE) \
    .option("kafka.sasl.mechanism", "PLAIN") \
    .option("kafka.security.protocol", "SASL_SSL") \
    .option("kafka.sasl.jaas.config", EH_SASL) \
    .option("failOnDataLoss", "false") \
    .load()

# COMMAND ----------

df_write = df.writeStream \
    .outputMode("append") \
    .format("console") \
    .queryName("preview") \
    .start()

# COMMAND ----------

ddf = df \
  .select( \
          get_json_object(df.value.cast("string"), "$.Temperature").alias("Temperature"), \
          get_json_object(df.value.cast("string"), "$.Timestamp").alias("Timestamp"))

display(ddf)		  

# COMMAND ----------

ddf = ddf.groupBy("Temperature").count()
ddf = ddf.orderBy(desc("count"))
display(ddf)

# COMMAND ----------



