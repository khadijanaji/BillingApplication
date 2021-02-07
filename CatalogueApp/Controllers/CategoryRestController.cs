using CatalogueApp.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CatalogueApp.Controllers{
    [Route("/api/categories")]
    public class CategoryRestController:Controller{
          public CatalogueDbRepository catalogueDbRepository { get; set; }
        private string topic = "facture";
        private ProducerConfig _config;
        public CategoryRestController(ProducerConfig _config, CatalogueDbRepository repository)
        {
            this._config = _config;
            this.catalogueDbRepository = repository;
        }
                
    
        [HttpGet]
        public IEnumerable<Category> FindAll(){
            return catalogueDbRepository.Categories;
        }
         [HttpGet("{Id}")]
        public Category GetOne(int Id){
            return catalogueDbRepository.Categories.FirstOrDefault(c=>c.CategoryID==Id);
        }
        [HttpGet("{Id}/products")]
        public IEnumerable<Product> products(int Id){
            Category category=catalogueDbRepository.Categories.Include(c=>c.Products).FirstOrDefault(c=>c.CategoryID==Id);
            return category.Products;
        }
    
         [HttpPost]
        public async Task<ActionResult> save([FromBody]Category category){
            catalogueDbRepository.Categories.Add(category);
            catalogueDbRepository.SaveChanges();
            string serializedCategory = JsonConvert.SerializeObject(category);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Add category : "+serializedCategory });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
        }
         [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id){
            Category category=catalogueDbRepository.Categories.FirstOrDefault(c=>c.CategoryID==Id);
            catalogueDbRepository.Remove(category);
            catalogueDbRepository.SaveChanges();
             string serializedCategory = JsonConvert.SerializeObject(category);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Delete category : " + serializedCategory });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult> Update(int Id,[FromBody]Category category){
            string serializedCategory_before = JsonConvert.SerializeObject(category);
            category.CategoryID=Id;
            catalogueDbRepository.Categories.Update(category);
            catalogueDbRepository.SaveChanges();
             string serializedCategory = JsonConvert.SerializeObject(category);
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { 
                    Value = "Update category from " + serializedCategory_before + " to "+ serializedCategory 
                });
                producer.Flush(TimeSpan.FromSeconds(10));
                return Ok(true);
            }
    
        }

    }
}