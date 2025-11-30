echo Starting heavy client
cd HeavyClient
mvn clean install
mvn exec:java -Dexec.mainClass="com.soc.testwsclient.TestWSClient.App
