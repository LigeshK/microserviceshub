using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

/// <summary>
/// Implement DiscountService inherting from DiscountProtoServiceBase, created by Visual studio when building the project
/// To implement the Discount Service use case, we need to override the default DiscountProtoServiceBase methods
/// </summary>
public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
{
    //When Grpc call comes in, we override the existing methods and execute the custom logic here
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        ///Get data from SqlLite DB
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
        //If coupon not found , create a new coupon object with no discount
        if (coupon is null)
            coupon = new Models.Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount", };

        logger.LogInformation("Discount for Product Name : {productName} Amount : {amount}", coupon.ProductName, coupon.Amount);

        //If coupon present , convert coupon object to couponmodel via Mapster and return the value
        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }
    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request"));

        //if data present , insert to database - sqlite
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Discount inserted for {ProductName}, Amount :{amount}", coupon.ProductName, coupon.Amount);

        //Once inserted, map the coupon to couponmodel and return
        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }
    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request"));

        //if data present , insert to database - sqlite
        dbContext.Coupons.Update(coupon);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Discount updated for {ProductName}, Amount :{amount}", coupon.ProductName, coupon.Amount);

        //Once inserted, map the coupon to couponmodel and return
        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }
    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        ///Get data from SqlLite DB
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
        //If coupon not found , create a new coupon object with no discount
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found"));
        dbContext.Coupons.Remove(coupon);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Discount deleted for {ProductName}", coupon.ProductName);
        return new DeleteDiscountResponse() { Success = true };
    }
}
