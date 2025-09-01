using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Configuration;

public class SagaConfiguration : IEntityTypeConfiguration<SagaEntity>
{
    public void Configure(EntityTypeBuilder<SagaEntity> builder)
    {
        builder.HasKey(p => p.CorrelationId);
        
        builder.Property(p => p.CorrelationId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(p => p.ReferenceDate)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.CurrentStep)
            .HasDefaultValue(null)
            .IsRequired(false);

        builder.Property(p => p.Payload)
            .HasColumnType("jsonb")
            .HasMaxLength(10000)
            .IsRequired(false);

        builder.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.Property<DateTime>("UpdatedAt")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Navigation(b => b.SagaEvents)
            .AutoInclude();

        builder.OwnsMany(b => b.SagaEvents, se =>
        {
            se.Property<int>("Id")
                .ValueGeneratedOnAdd();

            se.HasKey("Id");
            
            se.Property(p => p.OccuredAt)
                .HasDefaultValueSql("NOW()");

            se.Property(p => p.Status);

            se.Property(p => p.Step);
        });
    }
}