package kafkadotnetconsumer.demo;

import org.apache.kafka.clients.consumer.ConsumerRecord;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.util.logging.FileHandler;
import java.util.logging.Logger;

@Service
public class KafkaConsumer {
    @Autowired
    Logger logger;

    @Bean
    Logger initLogger() throws IOException{
        Logger logger=Logger.getLogger(this.getClass().getName());
        logger.addHandler(new FileHandler("log.xml"));
        logger.setUseParentHandlers(false);
        return logger;
    }

    @KafkaListener(topics = {"facture"},groupId = "consumer1")
    public void onMessage(ConsumerRecord<String,String> consumerRecord){
        System.out.println("Message ====> "+consumerRecord.value());
        logger.info(consumerRecord.value());
    }
}
