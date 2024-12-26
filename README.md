# Implementing Change Data Capture (CDC) from Postgres using Debezium

This project demonstrates how to implement Change Data Capture (CDC) from a PostgreSQL database using Debezium, Kafka, and a .NET application. The setup captures changes in the `transactions` table and publishes them to a Kafka topic.

## Requirements

- Docker and Docker Compose
- .NET 9.0 SDK
- Kafka UI for monitoring

## Running the Example

1. **Start the Services**: Use Docker Compose to start PostgreSQL, Kafka, and Debezium services. This setup includes a Kafka broker, a schema registry, and a Kafka UI for monitoring.

2. **Create the Debezium Connector**: Run the POST request in `debezium.http` to create a Debezium connector. This connector listens for changes in the `transactions` table and publishes them to the `transaction-changes` Kafka topic.

3. **Launch the .NET Application**: Start the .NET application that simulates transactions. This application inserts random transactions into the PostgreSQL database.

4. **Monitor Changes**: Use Kafka UI to monitor the `transaction-changes` topic and observe the changes captured by Debezium.
