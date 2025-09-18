using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public IGenericRepository<Category> _Category;
        public IGenericRepository<FoodType> _FoodType;
        public IGenericRepository<MenuItem> _MenuItem;
        public IGenericRepository<MenuItemFoodType> _MenuItemFoodType;
        public IGenericRepository<ApplicationUser> _ApplicationUser;
        public IGenericRepository<ShoppingCart> _ShoppingCart;
        public IGenericRepository<OrderHeader> _OrderHeader;
        public IGenericRepository<OrderDetails> _OrderDetails;
        public IGenericRepository<PromoCode> _PromoCode;

        public IGenericRepository<Category> Category
        {
            get
            {
                if (_Category == null)
                {
                    _Category = new GenericRepository<Category>(_dbContext);
                }
                return _Category;
            }
        }

        public IGenericRepository<FoodType> FoodType
        {
            get
            {
                if (_FoodType == null)
                {
                    _FoodType = new GenericRepository<FoodType>(_dbContext);
                }
                return _FoodType;
            }
        }

        public IGenericRepository<MenuItem> MenuItem
        {
            get
            {
                if (_MenuItem == null)
                {
                    _MenuItem = new GenericRepository<MenuItem>(_dbContext);
                }
                return _MenuItem;
            }
        }

        public IGenericRepository<MenuItemFoodType> MenuItemFoodType
        {
            get
            {
                if (_MenuItemFoodType == null)
                {
                    _MenuItemFoodType = new GenericRepository<MenuItemFoodType>(_dbContext);
                }
                return _MenuItemFoodType;
            }
        }

        public IGenericRepository<ApplicationUser> ApplicationUser
        {
            get
            {
                if (_ApplicationUser == null)
                {
                    _ApplicationUser = new GenericRepository<ApplicationUser>(_dbContext);
                }
                return _ApplicationUser;
            }
        }

        public IGenericRepository<ShoppingCart> ShoppingCart
        {
            get
            {
                if (_ShoppingCart == null)
                {
                    _ShoppingCart = new GenericRepository<ShoppingCart>(_dbContext);
                }
                return _ShoppingCart;
            }
        }

        public IGenericRepository<OrderHeader> OrderHeader
        {
            get
            {
                if (_OrderHeader == null)
                {
                    _OrderHeader = new GenericRepository<OrderHeader>(_dbContext);
                }
                return _OrderHeader;
            }
        }

        public IGenericRepository<OrderDetails> OrderDetails
        {
            get
            {
                if (_OrderDetails == null)
                {
                    _OrderDetails = new GenericRepository<OrderDetails>(_dbContext);
                }
                return _OrderDetails;
            }
        }

        public IGenericRepository<PromoCode> PromoCode
        {
            get
            {
                if (_PromoCode == null)
                {
                    _PromoCode = new GenericRepository<PromoCode>(_dbContext);
                }
                return _PromoCode;
            }
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
