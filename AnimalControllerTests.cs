using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            Console.WriteLine(response);
        }

    }
}
            
