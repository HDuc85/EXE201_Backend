using Data.Models;
using Service.Service;

namespace Service.Interface
{
    public interface IUnitOfWork
    {
        Repository<User> RepositoryUser { get; }
        Repository<UserStatusLog> RepositoryUserStatusLog { get; }
        Repository<Status> RepositoryStatus { get; }     
        Repository<Box> RepositoryBox { get; }
        Repository<BoxItem> RepositoryBoxItem { get; }
        Repository<BoxMedia> RepositoryBoxMedia { get; }
        Repository<BoxTag> RepositoryBoxTag { get; }
        Repository<Cart> RepositoryCart { get; }
        Repository<Feedback> RepositoryFeedback { get; }
        Repository<FeedbackMedia> RepositoryFeedbackMedia { get; }
        Repository<Media> RepositoryMedia { get; }
        Repository<MediaType> RepositoryMediaType { get; }
        Repository<Order> RepositoryOrder { get; }
        Repository<OrderItem> RepositoryOrderItem { get; }
        Repository<OrderStatus> RepositoryOrderStatus { get; }
        Repository<OrderStatusLog> RepositoryOrderStatusLog { get; }
        Repository<Product> RepositoryProduct { get; }
        Repository<ProductMedia> RepositoryProductMedia { get; }
        Repository<ProductTag> RepositoryProductTag { get; }
        Repository<ProductVariant> RepositoryProductVariant { get; }
        Repository<Tag> RepositoryTag { get; }
        Repository<TagValue> RepositoryTagValue { get; }
        Repository<Size> RepositorySize { get; }
        Repository<Color> RepositoryColor { get; }
        Repository<Brand> RepositoryBrand { get; }
        Repository<PaymentStatus> RepositoryPaymentStatus { get; }
        Repository<PaymentDetail> RepositoryPaymentDetail { get; }

        Task CommitAsync();
    }
}