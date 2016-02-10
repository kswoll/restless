using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Restless.Database;

namespace Restless.Database.Migrations
{
    [DbContext(typeof(RestlessDb))]
    partial class RestlessDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("Restless.Database.DbApiCall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApiId");

                    b.Property<byte[]>("Body");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiCall");
                });

            modelBuilder.Entity("Restless.Database.DbApiCallRequestHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApiCallId");

                    b.Property<int?>("ApiId");

                    b.Property<int?>("DbApiCallId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiCallRequestHeader");
                });

            modelBuilder.Entity("Restless.Database.DbApiCallResponseHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApiCallId");

                    b.Property<int?>("ApiId");

                    b.Property<int?>("DbApiCallId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiCallResponseHeader");
                });

            modelBuilder.Entity("Restless.Database.DbApiHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApiId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiHeader");
                });

            modelBuilder.Entity("Restless.Database.DbApiInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApiId");

                    b.Property<string>("DefaultValue");

                    b.Property<int>("InputType");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiInput");
                });

            modelBuilder.Entity("Restless.Database.DbApiItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CollectionId");

                    b.Property<DateTime>("Created");

                    b.Property<int>("Method");

                    b.Property<byte[]>("RequestBody");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiItem");
                });

            modelBuilder.Entity("Restless.Database.DbApiOutput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApiId");

                    b.Property<string>("Expression");

                    b.Property<string>("Name");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiOutput");
                });

            modelBuilder.Entity("Restless.Database.DbApiResponseComplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApiId");

                    b.Property<string>("ComplicationClass");

                    b.Property<string>("ComplicationData");

                    b.Property<int>("Priority");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ApiResponseVisualizer");
                });

            modelBuilder.Entity("Restless.Database.DbApiCall", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("ApiId");
                });

            modelBuilder.Entity("Restless.Database.DbApiCallRequestHeader", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("ApiId");

                    b.HasOne("Restless.Database.DbApiCall")
                        .WithMany()
                        .HasForeignKey("DbApiCallId");
                });

            modelBuilder.Entity("Restless.Database.DbApiCallResponseHeader", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("ApiId");

                    b.HasOne("Restless.Database.DbApiCall")
                        .WithMany()
                        .HasForeignKey("DbApiCallId");
                });

            modelBuilder.Entity("Restless.Database.DbApiHeader", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("ApiId");
                });

            modelBuilder.Entity("Restless.Database.DbApiInput", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("ApiId");
                });

            modelBuilder.Entity("Restless.Database.DbApiItem", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("CollectionId");
                });

            modelBuilder.Entity("Restless.Database.DbApiOutput", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("ApiId");
                });

            modelBuilder.Entity("Restless.Database.DbApiResponseComplication", b =>
                {
                    b.HasOne("Restless.Database.DbApiItem")
                        .WithMany()
                        .HasForeignKey("ApiId");
                });
        }
    }
}
