using CommonLayer;
using CommonLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepositoryLayers.Interface;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayers.Services
{
    public class CardRL : ICardRL
    {
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _SqlConnection;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CardRL> _logger;
        public CardRL(IConfiguration configuration, ApplicationDbContext dbContext, ILogger<CardRL> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
            _SqlConnection = new SqlConnection(_configuration["ConnectionStrings:DBSettingConnection"]);
        }

        public async Task<AddRatingResponse> AddRating(AddRatingRequest request)
        {
            AddRatingResponse response = new AddRatingResponse();
            response.IsSuccess = true;
            response.Message = "Add Rating Successfully";

            try
            {
                _logger.LogInformation($"AddRating Calling In CardRL....{JsonConvert.SerializeObject(request)}");
                /*string SqlQuery = @"  SELECT Rating FROM ProductDetails
                                      WHERE ProductID=@ProductID;
                                      ";*/

                var CartDetails = _dbContext.CardDetails
                                        .Where(X => X.CardID == request.CartID)
                                        .FirstOrDefault();

                CartDetails.Rating = request.Rating;
                _dbContext.CardDetails.Update(CartDetails);
                int Result = await _dbContext.SaveChangesAsync();
                if (Result <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Message : " + ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
            }

            return response;
        }

        public async Task<OrderProductResponse> PaymentGetway(PaymentGetwayRequest request)
        {
            OrderProductResponse response = new OrderProductResponse();
            response.IsSuccess = true;
            response.Message = "Payment Done Successfully";

            try
            {

                var CartDetail = await _dbContext
                    .CardDetails
                    .FirstOrDefaultAsync(x => x.CardID == request.CartID);

                CartDetail.UpiId = request.Upiid;
                CartDetail.PaymentType = request.PaymentType;
                CartDetail.CardNo = request.CardNo;
                CartDetail.IsPayment = true;

                _dbContext.CardDetails.Update(CartDetail);
                await _dbContext.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<AddToCardResponse> AddToCard(AddToCardRequest request)
        {
            AddToCardResponse response = new AddToCardResponse();
            response.Message = "Add To Cart Successfully.";
            response.IsSuccess = true;
            try
            {
                _logger.LogInformation($"AddToCard Calling In CardRL....{JsonConvert.SerializeObject(request)}");

                //string SqlQuery = @"INSERT INTO CardDetails (UserId, ProductID) VALUES (@UserId, @ProductID)";

                CardDetails cardDetails = new CardDetails()
                {
                    UserId = request.UserID,
                    ProductID = request.ProductID
                };

                await _dbContext.AddAsync(cardDetails);
                int Result = await _dbContext.SaveChangesAsync();
                if (Result <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong";
                }

            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message : " + ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<OrderProductResponse> CancleOrder(OrderProductRequest request)
        {
            OrderProductResponse response = new OrderProductResponse();
            response.Message = "Successfully Cancle Your Order";
            response.IsSuccess = true;
            try
            {
                _logger.LogInformation($"CancleOrder Calling In CardRL....{JsonConvert.SerializeObject(request)}");
                /*string SqlQuery = @"  UPDATE ProductDetails
                                      SET Quantity=Quantity+1
                                      WHERE ProductID=@ProductID;
                                        
                                      Update CardDetails 
                                      SET IsOrder=0
                                       WHERE CardID=@CartID";*/

                var ProductDetails = await _dbContext.ProductDetails.FindAsync(request.ProductID);

                if (ProductDetails == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product ID Not Found";
                    return response;
                }

                ProductDetails.Quantity = ProductDetails.Quantity + 1;
                int Result = await _dbContext.SaveChangesAsync();
                if (Result <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong";
                }

                var CartDetails = await _dbContext.CardDetails.FindAsync(request.CartID);

                if (CartDetails == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cart ID Not Found";
                    return response;
                }

                _dbContext.CardDetails.Remove(CartDetails);
                int DeleteResult = await _dbContext.SaveChangesAsync();
                if (DeleteResult <= 0)
                {
                    response.Message = "Something Went Wrong";
                }

                

                /*CartDetails.IsOrder = false;
                Result = await _dbContext.SaveChangesAsync();
                if (Result <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong";
                }*/


            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message : " + ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        // Inner Joins
        public async Task<GetAllCardDetailsResponse> GetAllCardDetails(GetAllCardDetailsRequest request)
        {
            GetAllCardDetailsResponse response = new GetAllCardDetailsResponse();
            response.Message = "Fetch All Carts Successfully";
            response.IsSuccess = true;
            try
            {
                _logger.LogInformation($"GetAllCardDetails Calling In CardRL....{JsonConvert.SerializeObject(request)}");

                /*string SqlQuery = @"
                                    SELECT Distinct C.CardID, P.ProductID, 
                                           P.InsertionDate, 
                                           P.ProductName, 
                                           P.ProductType , 
                                           P.ProductPrice, 
                                           P.ProductDetails, 
                                           P.ProductCompany, 
                                           P.Quantity, 
                                           P.ProductImageUrl,
                                           P.PublicID,
                                           P.IsArchive, 
                                           P.IsActive,
                                           (SELECT COUNT(*) FROM CardDetails WHERE IsOrder=0 AND UserID=@UserID) AS TotalRecord
                                    FROM CardDetails C 
                                    INNER JOIN ProductDetails P
                                    On C.ProductID = P.ProductID
                                    WHERE IsOrder=0 AND UserID=@UserID
                                    ORDER BY P.InsertionDate DESC
                                    OFFSET @Offset ROWS FETCH NEXT @NumberOfRecordPerPage ROWS ONLY;";*/

                var Result = (from c in _dbContext.CardDetails
                              join P in _dbContext.ProductDetails
                              on c.ProductID equals P.ProductID
                              where c.IsOrder == false && c.UserId == request.UserID
                              select new
                              {
                                  CardID = c.CardID,
                                  UserID = c.UserId,
                                  ProductID = P.ProductID,
                                  InsertionDate = P.InsertionDate,
                                  ProductName = P.ProductName,
                                  ProductType = P.ProductType,
                                  ProductPrice = P.ProductPrice,
                                  ProductDetails = P.ProductDetail,
                                  ProductCompany = P.ProductCompany,
                                  Quantity = P.Quantity,
                                  ProductImageUrl = P.ProductImageUrl,
                                  PublicID = P.PublicId,
                                  IsArchive = P.IsArchive,
                                  IsActive = P.IsActive
                              })
                              .OrderByDescending(X => X.InsertionDate)
                              .Skip(request.NumberOfRecordPerPage * (request.PageNumber - 1))
                              .Take(request.NumberOfRecordPerPage)
                              .ToList();

                if (Result.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Carts Not Found";
                    return response;
                }

                response.data = new List<GetAllCardDetails>();
                foreach (var data in Result)
                {
                    var UserData = _dbContext.CustomerDetails
                                                        .Where(X => X.UserID == data.UserID)
                                                        .FirstOrDefault();

                    GetAllCardDetails Cartdata = new GetAllCardDetails();
                    Cartdata.CartID = data.CardID;
                    Cartdata.FullName = UserData != null ? String.IsNullOrEmpty(UserData.FullName) ? "" : UserData.FullName : "";
                    Cartdata.InsertionDate = Convert.ToDateTime(data.InsertionDate).ToString("dddd, dd-MM-yyyy, HH:mm tt");
                    Cartdata.IsActive = data.IsActive;
                    Cartdata.IsArchive = data.IsArchive;
                    Cartdata.ProductCompany = data.ProductCompany;
                    Cartdata.ProductDetails = data.ProductDetails;
                    Cartdata.ProductID = data.ProductID;
                    Cartdata.ProductImageUrl = data.ProductImageUrl;
                    Cartdata.ProductName = data.ProductName;
                    Cartdata.ProductPrice = data.ProductPrice;
                    Cartdata.ProductType = data.ProductType;
                    Cartdata.PublicID = data.PublicID;
                    Cartdata.Quantity = data.Quantity;
                    response.data.Add(Cartdata);
                }

                response.TotalRecords = _dbContext.CardDetails
                                        .Where(X => X.IsOrder == false)
                                        .Count();
                response.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                response.CurrentPage = request.PageNumber;
            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message : " + ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        // Inner Joins
        public async Task<GetOrderProductResponse> GetOrderProduct(GetOrderProductRequest request)
        {
            GetOrderProductResponse response = new GetOrderProductResponse();
            response.Message = "Fetch Order List Successfully";
            response.IsSuccess = true;
            try
            {
                _logger.LogInformation($"GetOrderProduct Calling In CardRL....{JsonConvert.SerializeObject(request)}");

                string SqlQuery = string.Empty;

                if (request.UserID != -1)
                {
                    /*SqlQuery = @"
                                    SELECT Distinct C.CardID, (SELECT Distinct U.FullName FROM CustomerDetails U WHERE U.UserID = C.UserId) AS FullName, 
                                           P.ProductID, 
                                           P.InsertionDate, 
                                           P.ProductName, 
                                           P.ProductType , 
                                           P.ProductPrice, 
                                           P.ProductDetails, 
                                           P.ProductCompany, 
                                           P.Quantity, 
                                           P.ProductImageUrl,
                                           P.PublicID,
                                           P.IsArchive, 
                                           P.IsActive,
                                           (SELECT COUNT(*) FROM CardDetails WHERE IsOrder=1 AND UserID=@UserID) AS TotalRecord
                                    FROM CardDetails C 
                                    INNER JOIN ProductDetails P
                                    On C.ProductID = P.ProductID
                                    WHERE IsOrder=1 AND UserID=@UserID
                                    ORDER BY P.InsertionDate DESC
                                    OFFSET @Offset ROWS FETCH NEXT @NumberOfRecordPerPage ROWS ONLY;";*/


                    var Result = (from c in _dbContext.CardDetails
                                  join P in _dbContext.ProductDetails
                                  on c.ProductID equals P.ProductID
                                  where c.IsOrder == true && c.UserId == request.UserID
                                  select new
                                  {
                                      CardID = c.CardID,
                                      UserID = c.UserId,
                                      ProductID = P.ProductID,
                                      InsertionDate = P.InsertionDate,
                                      ProductName = P.ProductName,
                                      ProductType = P.ProductType,
                                      ProductPrice = P.ProductPrice,
                                      ProductDetails = P.ProductDetail,
                                      ProductCompany = P.ProductCompany,
                                      Quantity = P.Quantity,
                                      ProductImageUrl = P.ProductImageUrl,
                                      PublicID = P.PublicId,
                                      IsArchive = P.IsArchive,
                                      IsActive = P.IsActive, 
                                      IsPayment=c.IsPayment,  
                                      Rating=c.Rating,    
                                      PaymentType=c.PaymentType,     
                                      CardNo=c.CardNo,   
                                      UpiID=c.UpiId
                                  })
                              .OrderByDescending(X => X.InsertionDate)
                              .Skip(request.NumberOfRecordPerPage * (request.PageNumber - 1))
                              .Take(request.NumberOfRecordPerPage)
                              .ToList();

                    if (Result.Count == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Order Not Found";
                        return response;
                    }

                    response.data = new List<GetAllCardDetails>();
                    foreach (var data in Result)
                    {
                        var UserData = _dbContext.CustomerDetails
                                                        .Where(X => X.UserID == data.UserID)
                                                        .FirstOrDefault();

                        GetAllCardDetails Cartdata = new GetAllCardDetails();
                        Cartdata.CartID = data.CardID;
                        Cartdata.FullName = UserData != null ? String.IsNullOrEmpty(UserData.FullName) ? "" : UserData.FullName : "";
                        Cartdata.InsertionDate = Convert.ToDateTime(data.InsertionDate).ToString("dddd, dd-MM-yyyy, HH:mm tt");
                        Cartdata.IsActive = data.IsActive;
                        Cartdata.IsArchive = data.IsArchive;
                        Cartdata.ProductCompany = data.ProductCompany;
                        Cartdata.ProductDetails = data.ProductDetails;
                        Cartdata.ProductID = data.ProductID;
                        Cartdata.ProductImageUrl = data.ProductImageUrl;
                        Cartdata.ProductName = data.ProductName;
                        Cartdata.ProductPrice = data.ProductPrice;
                        Cartdata.ProductType = data.ProductType;
                        Cartdata.PublicID = data.PublicID;
                        Cartdata.Quantity = data.Quantity;
                        Cartdata.IsPayment = data.IsPayment;
                        Cartdata.Rating = data.Rating;
                        Cartdata.PaymentType = data.PaymentType;
                        Cartdata.CardNo = data.CardNo; 
                        Cartdata.UpiID=data.UpiID;
                        response.data.Add(Cartdata);
                    }
                }
                else
                {
                    /*SqlQuery = @"
                                    SELECT Distinct C.CardID, (SELECT Distinct U.FullName FROM CustomerDetails U WHERE U.UserID = C.UserId) AS FullName,
                                           P.ProductID, 
                                           P.InsertionDate, 
                                           P.ProductName, 
                                           P.ProductType , 
                                           P.ProductPrice, 
                                           P.ProductDetails, 
                                           P.ProductCompany, 
                                           P.Quantity, 
                                           P.ProductImageUrl,
                                           P.PublicID,
                                           P.IsArchive, 
                                           P.IsActive,
                                           (SELECT COUNT(*) FROM CardDetails WHERE IsOrder=1) AS TotalRecord
                                    FROM CardDetails C 
                                    INNER JOIN ProductDetails P
                                    On C.ProductID = P.ProductID
                                    WHERE IsOrder=1
                                    ORDER BY P.InsertionDate DESC
                                    OFFSET @Offset ROWS FETCH NEXT @NumberOfRecordPerPage ROWS ONLY;";*/

                    var Result = (from c in _dbContext.CardDetails
                                  join P in _dbContext.ProductDetails
                                  on c.ProductID equals P.ProductID
                                  where c.IsOrder == true
                                  select new
                                  {
                                      CardID = c.CardID,
                                      UserID = c.UserId,
                                      ProductID = P.ProductID,
                                      InsertionDate = P.InsertionDate,
                                      ProductName = P.ProductName,
                                      ProductType = P.ProductType,
                                      ProductPrice = P.ProductPrice,
                                      ProductDetails = P.ProductDetail,
                                      ProductCompany = P.ProductCompany,
                                      Quantity = P.Quantity,
                                      ProductImageUrl = P.ProductImageUrl,
                                      PublicID = P.PublicId,
                                      IsArchive = P.IsArchive,
                                      IsActive = P.IsActive
                                  })
                              .OrderByDescending(X => X.InsertionDate)
                              .Skip(request.NumberOfRecordPerPage * (request.PageNumber - 1))
                              .Take(request.NumberOfRecordPerPage)
                              .ToList();


                    if (Result.Count == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Order Not Found";
                        return response;
                    }

                    response.data = new List<GetAllCardDetails>();
                    foreach (var data in Result)
                    {
                        var UserData = _dbContext.CustomerDetails
                                                        .Where(X => X.UserID == data.UserID)
                                                        .FirstOrDefault();

                        GetAllCardDetails Cartdata = new GetAllCardDetails();

                        Cartdata.CartID = data.CardID;
                        Cartdata.FullName = UserData != null ? String.IsNullOrEmpty(UserData.FullName) ? "" : UserData.FullName : "";
                        Cartdata.InsertionDate = Convert.ToDateTime(data.InsertionDate).ToString("dddd, dd-MM-yyyy, HH:mm tt");
                        Cartdata.IsActive = data.IsActive;
                        Cartdata.IsArchive = data.IsArchive;
                        Cartdata.ProductCompany = data.ProductCompany;
                        Cartdata.ProductDetails = data.ProductDetails;
                        Cartdata.ProductID = data.ProductID;
                        Cartdata.ProductImageUrl = data.ProductImageUrl;
                        Cartdata.ProductName = data.ProductName;
                        Cartdata.ProductPrice = data.ProductPrice;
                        Cartdata.ProductType = data.ProductType;
                        Cartdata.PublicID = data.PublicID;
                        Cartdata.Quantity = data.Quantity;
                        response.data.Add(Cartdata);
                    }
                }

                response.TotalRecords = _dbContext.CardDetails.Where(X => X.IsOrder == true).Count();
                response.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                response.CurrentPage = request.PageNumber;

            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message : " + ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<OrderProductResponse> OrderProduct(OrderProductRequest request)
        {
            OrderProductResponse response = new OrderProductResponse();
            response.Message = "Successfully Order Your Product";
            response.IsSuccess = true;
            try
            {
                _logger.LogInformation($"OrderProduct Calling In CardRL....{JsonConvert.SerializeObject(request)}");

                /*string SqlQuery = @"  UPDATE ProductDetails
                                      SET Quantity=Quantity-1
                                      WHERE ProductID=@ProductID;
                                        
                                      Update CardDetails 
                                      SET IsOrder=1 
                                       WHERE CardID=@CartID";*/

                var ProductDetails = await _dbContext.ProductDetails.FindAsync(request.ProductID);

                if (ProductDetails == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product ID Not Found";
                    return response;
                }

                ProductDetails.Quantity = ProductDetails.Quantity - 1;
                int Result = await _dbContext.SaveChangesAsync();
                if (Result <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong";
                }

                var CartDetails = await _dbContext.CardDetails.FindAsync(request.CartID);

                if (CartDetails == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cart ID Not Found";
                    return response;
                }

                CartDetails.IsOrder = true;
                Result = await _dbContext.SaveChangesAsync();
                if (Result <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong";
                }

            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message : " + ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<RemoveCardResponse> RemoveCartProduct(RemoveCardRequest request)
        {
            RemoveCardResponse response = new RemoveCardResponse();
            response.Message = "Remove Cart Successfully.";
            response.IsSuccess = true;
            try
            {
                _logger.LogInformation($"RemoveCartProduct Calling In CardRL....{JsonConvert.SerializeObject(request)}");

                if (_SqlConnection != null && _SqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _SqlConnection.OpenAsync();
                }

                string SqlQuery = @"DELETE FROM CardDetails WHERE CardID = @CartID";

                var CardDetails = await _dbContext.CardDetails.FindAsync(request.CartID);
                if (CardDetails == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cart Record Not Found";
                }

                _dbContext.CardDetails.Remove(CardDetails);
                int DeleteResult = await _dbContext.SaveChangesAsync();
                if (DeleteResult <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Something went Wrong";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message : " + ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }
    }
}
