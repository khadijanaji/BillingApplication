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
    [Route("/api/products")]
    public class ProductRestController:Controller{
       
        public CatalogueDbRepository catalogueDbRepository { get; set; }
        private string topic = "facture";
        private ProducerConfig _config;

        public ProductRestController(ProducerConfig _config, CatalogueDbRepository repository)
        {
            this._config = _config;
            this.catalogueDbRepository = repository;
        }
        [HttpGet]
        public IEnumerable<Product> FindAll(){
            return catalogueDbRepository.Products.Include(p=>p.category);
        }
        [HttpGet("search")]
        public IEnumerable<Product> search(string kw){
            return catalogueDbRepository
            .Products
            .Include(p=>p.category)
            .Where(p=>p.Name.Contains(kw));
        }
          [HttpGet("paginate")]
        public IEnumerable<Product> page(int page=0,int size=1){
            int skipValue=(page-1)*size;
            return catalogueDbRepository
            .Products
            .Include(p=>p.category)
            .Skip(skipValue)
            .Take(size);
        }
         [HttpGet("{Id}")]
        public Product GetOne(int Id){
            return catalogueDbRepository.Products.Include(p=>p.category).FirstOrDefault(p=>p.ProductID==Id);
        }
         [HttpPost]
        public async Task<ActionResult> save([FromBody]Product product){
            catalogueDbRepository.Products.Add(product);
            catalogueDbRepository.SaveChanges();
            string serializedProduct = JsonConvert.SerializeObject(product);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Add product : " + serializedProduct });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
        }
         [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id){
            Product product=catalogueDbRepository.Products.FirstOrDefault(p=>p.ProductID==Id);
            catalogueDbRepository.Remove(product);
            catalogueDbRepository.SaveChanges();
            string serializedProduct = JsonConvert.SerializeObject(product);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Delete product : " + serializedProduct });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }

        }
        [HttpPut("{Id}")]
        public async Task<ActionResult> Update(int Id,[FromBody]Product product){
            product.ProductID=Id;
            catalogueDbRepository.Products.Update(product);
            catalogueDbRepository.SaveChanges();
            string serializedProduct_before = JsonConvert.SerializeObject(product);
             string serializedProduct = JsonConvert.SerializeObject(product);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { 
                    Value = "Update product from " + serializedProduct_before + " to " + serializedProduct 
                });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
    
        }

    }
}