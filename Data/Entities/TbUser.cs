using Microsoft.EntityFrameworkCore;

namespace WebApplication.Data.Entities
{
    public partial class TbUser
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// ConfigureModelBuilder foi criado para retirar a maior parte do código do arquivo AppDbContext, 
        /// deixando o intelissense mais leve e mais rápido.
        /// A configuração de cada 'Entity' deve ser feita dentro do próprio arquivo.
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_TbUser");
                
                entity.ToTable("TbUser");
                
                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Name")
                    .HasColumnType("nvarchar(100)");
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("Email")
                    .HasColumnType("nvarchar(255)");
                
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("PasswordHash")
                    .HasColumnType("nvarchar(255)");
                
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("CreatedAt")
                    .HasColumnType("datetime2");
                
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("UpdatedAt")
                    .HasColumnType("datetime2");
                
                entity.Property(e => e.IsActive)
                    .HasColumnName("IsActive")
                    .HasColumnType("bit");
                
                // Index único no email
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_TbUser_Email");
            });
        }
    }
}
