using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TameShop.Controllers;
using TameShop.Data;
using TameShop.Models;

namespace TameShop.Tests
{
    [TestClass]
    public sealed class AnimalControllerTests
    {
        private TameShopDbContext _context; 
        private AnimalController _animalController;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TameShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;
            _context = new TameShopDbContext(options); 
            _animalController = new AnimalController(_context);

            var sheep = new Animal
            {
                Id = 1,
                Name = "Sheep",
                Category = "Farm",
                Description = "A lovely sheep",
                ImageUrl = "https://example.com/sheep.jpg",
                Price = 49.99M,
            };

            _context.Animals.Add(sheep);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task AnimalGet()  
        {
            var response = await _animalController.GetAnimal(1);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task AnimalAddPig()
        {
            var pig = new Animal
            {
                Id = 2,
                Name = "Pig",
                Category = "Farm",
                Description = "A cute pig",
                ImageUrl = "https://example.com/pig.jpg",
                Price = 39.99M,
            };
            await _context.Animals.AddAsync(pig);
            await _context.SaveChangesAsync();
            var response = await _animalController.GetAnimal(2);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task AnimalDeletePig()
        {
            var pig = new Animal
            {
                Id = 2,
                Name = "Pig",
                Category = "Farm",
                Description = "A cute pig",
                ImageUrl = "https://example.com/pig.jpg",
                Price = 39.99M,
            };
            await _context.Animals.AddAsync(pig);
            await _context.SaveChangesAsync();
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == 2);
            if (animal != null)
            {
                await _animalController.DeleteAnimal(animal.Id);
            }
            var response = await _animalController.GetAnimal(2);
            
            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult));
        }
    }
}
            
