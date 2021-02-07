using CatalogueApp.Models;
using System;
namespace CatalogueApp.Services{
    public static class DbInit {
        public static void initData(CatalogueDbRepository catalogueDb){
            Console.WriteLine("Data Initialization");
            catalogueDb.Categories.Add(new Category{Name="Ordinateurs"});
            catalogueDb.Categories.Add(new Category{Name="Imprimantes"});
            catalogueDb.Categories.Add(new Category{Name="Smartphones"});
            catalogueDb.Products.Add(new Product{Name="Ord HP 450",Price=5500,CategoryID=1});
            catalogueDb.Products.Add(new Product{Name="Ord Comp 220",Price=10050,CategoryID=1});
            catalogueDb.Products.Add(new Product{Name="Mac book pro",Price=45000,CategoryID=1});
            catalogueDb.Products.Add(new Product{Name="Imprimante Epson 458",Price=450,CategoryID=2});
            catalogueDb.Products.Add(new Product{Name="Sumsung s9",Price=4050,CategoryID=3});
            catalogueDb.Products.Add(new Product{Name="Iphone 12 pro",Price=11450,CategoryID=3});
            catalogueDb.Costumers.Add(new Costumer{Name="mohammed"});
            catalogueDb.Costumers.Add(new Costumer{Name="khadija",Email="khadija@gmail.com"});
            catalogueDb.Costumers.Add(new Costumer{Name="sara",Email="sara@gamil.com"});
            catalogueDb.SaveChanges();


        }
     }
}