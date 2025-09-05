using Microsoft.EntityFrameworkCore;
using TameShop.Controllers;
using TameShop.Data;
using TameShop.Repositories.Interfaces; 
using TameShop.Services.Interfaces;
using TameShop.Models;

namespace TameShop.Tests;

[TestClass]
public class CartsControllerTest
{
    // Nested dependencies are horrible
    // but for the sake of this example, we'll keep it simple
    // TameShopDbContext -> ICartRepository -> ICartsService -> CartsController
    private TameShopDbContext _context;
    private ICartRepository _cartRepository;
    private ICartsService _cartsService;
    private CartsController _cartsController;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TameShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new TameShopDbContext(options);
        _cartRepository = new Repositories.Implementations.CartRepository(_context);
        _cartsService = new Services.Implementations.CartsService(_cartRepository);
        _cartsController = new CartsController(_cartsService);

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
}
