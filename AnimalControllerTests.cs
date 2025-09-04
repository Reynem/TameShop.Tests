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
        private  TameShopDbContext _context; 
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
                Price = 19.99M,
            };

            var pig = new Animal
            {
                Id = 2,
                Name = "Pig",
                Category = "Farm",
                Description = "A cute pig",
                ImageUrl = "https://example.com/pig.jpg",
                Price = 39.99M,
            };

            var cat = new Animal
            {
                Id = 3,
                Name = "Cat",
                Category = "Pet",
                Description = "A playful cat",
                ImageUrl = "https://example.com/cat.jpg",
                Price = 59.99M,
            };

            _context.Animals.AddRange(sheep, pig, cat);
            _context.SaveChanges();
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
        public async Task AnimalAddCow()
        {
            var cow = new Animal
            {
                Id = 4,
                Name = "Cow",
                Category = "Farm",
                Description = "A big cow",
                ImageUrl = "https://example.com/cow.jpg",
                Price = 79.99M,
            };
            await _context.Animals.AddAsync(cow);
            await _context.SaveChangesAsync();
            var response = await _animalController.GetAnimal(4);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task AnimalDeleteCow()
        {
            var cow = new Animal
            {
                Id = 4,
                Name = "Cow",
                Category = "Farm",
                Description = "A big cow",
                ImageUrl = "https://example.com/cow.jpg",
                Price = 79.99M,
            };
            await _context.Animals.AddAsync(cow);
            await _context.SaveChangesAsync();
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == 4);
            if (animal != null)
            {
                await _animalController.DeleteAnimal(animal.Id);
            }
            var response = await _animalController.GetAnimal(4);
            
            Assert.IsInstanceOfType<NotFoundObjectResult>(response);
        }

        [TestMethod]
        public async Task SearchAnimals_Found()
        {
            var result = await _animalController.SearchAnimals("sheep") as OkObjectResult;

            Assert.IsNotNull(result);
            var animals = result.Value as List<Animal>;
            Assert.IsNotNull(animals);
            Assert.AreEqual(1, animals.Count);
            Assert.AreEqual("Sheep", animals[0].Name);
        }

        [TestMethod]
        public async Task SearchAnimals_NotFound()
        {
            var result = await _animalController.SearchAnimals("dog");

            Assert.IsInstanceOfType<NotFoundObjectResult>(result);
        }

        [TestMethod]
        public async Task SearchAnimals_BadRequest()
        {
            var result = await _animalController.SearchAnimals("") as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Name cannot be empty.", result.Value);
        }

        [TestMethod]
        public async Task FilterByPrice_FoundAsc()
        {
            var result = await _animalController.FilterByPrice(30, 60, "asc", 1, 10) as OkObjectResult;

            Assert.IsNotNull(result);
            var animals = result.Value as List<Animal>;
            Assert.IsNotNull(animals);
            Assert.AreEqual(2, animals.Count);
            Assert.AreEqual("Pig", animals[0].Name); // cheaper
            Assert.AreEqual("Cat", animals[1].Name); // more expensive
        }

        [TestMethod]
        public async Task FilterByPrice_FoundDesc()
        {
            var result = await _animalController.FilterByPrice(30, 60, "desc", 1, 10) as OkObjectResult;

            Assert.IsNotNull(result);
            var animals = result.Value as List<Animal>;
            Assert.IsNotNull(animals);
            Assert.AreEqual(2, animals.Count);
            Assert.AreEqual("Cat", animals[0].Name); // more expensive
            Assert.AreEqual("Pig", animals[1].Name); // cheaper
        }

        [TestMethod]
        public async Task FilterByPrice_NotFound()
        {
            var result = await _animalController.FilterByPrice(100, 200);

            Assert.IsInstanceOfType<NotFoundObjectResult>(result);
        }

        [TestMethod]
        public async Task FilterByPrice_BadRequest()
        {
            var result = await _animalController.FilterByPrice(60, 30);

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        public async Task GetAnimalsPaged_Valid()
        {
            var result = await _animalController.GetAnimalsPaged(1, 2) as OkObjectResult;

            Assert.IsNotNull(result);
            dynamic data = result.Value!;
            Assert.AreEqual(3, data.TotalCount);
            Assert.AreEqual(1, data.Page);
            Assert.AreEqual(2, data.PageSize);
            Assert.AreEqual(2, ((List<Animal>)data.Data).Count);
        }

        [TestMethod]
        public async Task GetAnimalsPaged_BadRequest()
        {
            var result = await _animalController.GetAnimalsPaged(0, 2);

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
        }
    }
}
            
