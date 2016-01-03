using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Restless.Database;

namespace Restless.Database.Migrations
{
    [DbContext(typeof(RestlessDb))]
    [Migration("20160102194850_InitialDb")]
    partial class InitialDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("Restless.Database.DbApi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Body");

                    b.Property<string>("HttpMethod")
                        .IsRequired();

                    b.Property<string>("ResponseVisualizer");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Api");
                });

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

            modelBuilder.Entity("Restless.Database.DbApiCall", b =>
                {
                    b.HasOne("Restless.Database.DbApi")
                        .WithMany()
                        .HasForeignKey("ApiId");
                });

            modelBuilder.Entity("Restless.Database.DbApiCallRequestHeader", b =>
                {
                    b.HasOne("Restless.Database.DbApi")
                        .WithMany()
                        .HasForeignKey("ApiId");

                    b.HasOne("Restless.Database.DbApiCall")
                        .WithMany()
                        .HasForeignKey("DbApiCallId");
                });

            modelBuilder.Entity("Restless.Database.DbApiCallResponseHeader", b =>
                {
                    b.HasOne("Restless.Database.DbApi")
                        .WithMany()
                        .HasForeignKey("ApiId");

                    b.HasOne("Restless.Database.DbApiCall")
                        .WithMany()
                        .HasForeignKey("DbApiCallId");
                });

            modelBuilder.Entity("Restless.Database.DbApiHeader", b =>
                {
                    b.HasOne("Restless.Database.DbApi")
                        .WithMany()
                        .HasForeignKey("ApiId");
                });
        }
    }
}
