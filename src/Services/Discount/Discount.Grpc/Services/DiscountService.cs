using Grpc.Core;

namespace Discount.Grpc.Services;

/// <summary>
/// Implement DiscountService inherting from DiscountProtoServiceBase, created by Visual studio when building the project
/// To implement the Discount Service use case, we need to override the default DiscountProtoServiceBase methods
/// </summary>
public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    //When Grpc call comes in, we override the exisitng methods and execute the custom logic here
    public override Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        return base.GetDiscount(request, context);
    }
    public override Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        return base.CreateDiscount(request, context);
    }
    public override Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        return base.UpdateDiscount(request, context);
    }
    public override Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        return base.DeleteDiscount(request, context);
    }
}
