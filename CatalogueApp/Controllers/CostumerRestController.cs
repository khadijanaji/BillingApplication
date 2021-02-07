using CatalogueApp.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogueApp.Controllers{
    [Route("/api/costumers")]
    public class CostumerRestController:Controller{
        
        public CatalogueDbRepository catalogueDbRepository { get; set; }
        private string topic = "facture";
        private ProducerConfig _config;

        public CostumerRestController(ProducerConfig _config, CatalogueDbRepository repository)
        {
            this._config = _config;
            this.catalogueDbRepository = repository;
        }

        [HttpGet]
        public IEnumerable<Costumer> FindAll(){
            return catalogueDbRepository.Costumers;
        }
        [HttpGet("search")]
        public IEnumerable<Costumer> search(string kw){
            return catalogueDbRepository
            .Costumers
            .Where(c=>c.Name.Contains(kw));
        }
          [HttpGet("paginate")]
        public IEnumerable<Costumer> page(int page=0,int size=1){
            int skipValue=(page-1)*size;
            return catalogueDbRepository
            .Costumers
            .Skip(skipValue)
            .Take(size);
        }
         [HttpGet("{Id}")]
        public Costumer GetOne(int Id){
            return catalogueDbRepository.Costumers.FirstOrDefault(c=>c.CostumerID==Id);
        }
         [HttpPost]
        public async Task<ActionResult> save([FromBody]Costumer costumer){
            catalogueDbRepository.Costumers.Add(costumer);
            catalogueDbRepository.SaveChanges();
            string serializedCustomer = JsonConvert.SerializeObject(costumer);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Add customer : " + serializedCustomer });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
        }
         [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id){
            Costumer costumer=catalogueDbRepository.Costumers.FirstOrDefault(c=>c.CostumerID==Id);
            catalogueDbRepository.Remove(costumer);
            catalogueDbRepository.SaveChanges();
            string serializedCustomer = JsonConvert.SerializeObject(costumer);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Delete customer : " + serializedCustomer });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult> Update(int Id,[FromBody]Costumer costumer){
            costumer.CostumerID=Id;
            catalogueDbRepository.Costumers.Update(costumer);
            catalogueDbRepository.SaveChanges();
            string serializedCustomer_before = JsonConvert.SerializeObject(costumer);
            string serializedCustomer = JsonConvert.SerializeObject(costumer);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { 
                    Value = "Update customer from " + serializedCustomer_before + " to " + serializedCustomer 
                });
                object p = producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
    
        }

    }
}