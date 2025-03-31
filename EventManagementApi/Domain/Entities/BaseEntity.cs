namespace EventManagementApi.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } // Oluşturma zamanı
        public DateTime? UpdatedAt { get; set; } // Güncelleme zamanı, isteğe bağlı
        public string CreatedBy { get; set; } // Kaydın kim tarafından oluşturulduğunu takip eder
        public string UpdatedBy { get; set; } // Kaydın kim tarafından güncellendiğini takip eder
        public bool IsDeleted { get; set; } // Soft delete için

        // Bu metodu entity üzerinde kullanacağız
        public void SetAuditFields(string userId)
        {
            CreatedBy = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public void UpdateAuditFields(string userId)
        {
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
