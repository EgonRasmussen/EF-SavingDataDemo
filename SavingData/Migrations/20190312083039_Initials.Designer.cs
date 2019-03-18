﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SavingData;

namespace SavingData.Migrations
{
    [DbContext(typeof(BloggingContext))]
    [Migration("20190312083039_Initials")]
    partial class Initials
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SavingData.Models.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("OwnerId");

                    b.Property<int?>("Rating");

                    b.Property<string>("Url");

                    b.HasKey("BlogId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Blogs");

                    b.HasData(
                        new
                        {
                            BlogId = 1,
                            OwnerId = 1,
                            Url = "http://blog1.com"
                        },
                        new
                        {
                            BlogId = 2,
                            OwnerId = 2,
                            Url = "http://blog2.com"
                        },
                        new
                        {
                            BlogId = 3,
                            OwnerId = 3,
                            Url = "http://blog3.com"
                        },
                        new
                        {
                            BlogId = 4,
                            Url = "http://blog5.com"
                        });
                });

            modelBuilder.Entity("SavingData.Models.Person", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int?>("PhotoId");

                    b.HasKey("PersonId");

                    b.ToTable("Person");

                    b.HasData(
                        new
                        {
                            PersonId = 1,
                            Name = "Person 1"
                        },
                        new
                        {
                            PersonId = 2,
                            Name = "Person 2"
                        },
                        new
                        {
                            PersonId = 3,
                            Name = "Person 3"
                        },
                        new
                        {
                            PersonId = 4,
                            Name = "Person 4"
                        },
                        new
                        {
                            PersonId = 5,
                            Name = "Person 5"
                        },
                        new
                        {
                            PersonId = 6,
                            Name = "Person 6"
                        });
                });

            modelBuilder.Entity("SavingData.Models.PersonPhoto", b =>
                {
                    b.Property<int>("PersonPhotoId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Caption");

                    b.Property<int?>("PersonId");

                    b.Property<byte[]>("Photo");

                    b.HasKey("PersonPhotoId");

                    b.HasIndex("PersonId")
                        .IsUnique()
                        .HasFilter("[PersonId] IS NOT NULL");

                    b.ToTable("PersonPhoto");

                    b.HasData(
                        new
                        {
                            PersonPhotoId = 1,
                            Caption = "PersonPhoto 1",
                            PersonId = 1,
                            Photo = new byte[] { 65, 66, 67 }
                        },
                        new
                        {
                            PersonPhotoId = 2,
                            Caption = "PersonPhoto 2",
                            PersonId = 2,
                            Photo = new byte[] { 68, 69, 70 }
                        },
                        new
                        {
                            PersonPhotoId = 3,
                            Caption = "PersonPhoto 3",
                            PersonId = 3,
                            Photo = new byte[] { 71, 72, 73 }
                        },
                        new
                        {
                            PersonPhotoId = 4,
                            Caption = "PersonPhoto 4",
                            PersonId = 4,
                            Photo = new byte[] { 74, 75, 76 }
                        },
                        new
                        {
                            PersonPhotoId = 5,
                            Caption = "PersonPhoto 5",
                            PersonId = 5,
                            Photo = new byte[] { 77, 78, 79 }
                        });
                });

            modelBuilder.Entity("SavingData.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuthorId");

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<int>("Rating");

                    b.Property<string>("Title");

                    b.HasKey("PostId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BlogId");

                    b.ToTable("Posts");

                    b.HasData(
                        new
                        {
                            PostId = 1,
                            AuthorId = 1,
                            BlogId = 1,
                            Content = "Dette er Post 1 i Blog 1",
                            Rating = 2,
                            Title = "Post 1"
                        },
                        new
                        {
                            PostId = 2,
                            AuthorId = 4,
                            BlogId = 1,
                            Content = "Dette er Post 2 i Blog 1",
                            Rating = 3,
                            Title = "Post 2"
                        },
                        new
                        {
                            PostId = 3,
                            AuthorId = 4,
                            BlogId = 1,
                            Content = "Dette er Post 3 i Blog 1",
                            Rating = 4,
                            Title = "Post 3"
                        },
                        new
                        {
                            PostId = 4,
                            AuthorId = 5,
                            BlogId = 2,
                            Content = "Dette er post 1 i Blog 2",
                            Rating = 0,
                            Title = "Post 1"
                        },
                        new
                        {
                            PostId = 5,
                            AuthorId = 6,
                            BlogId = 2,
                            Content = "Dette er post 2 i Blog 2",
                            Rating = 0,
                            Title = "Post 2"
                        },
                        new
                        {
                            PostId = 6,
                            BlogId = 3,
                            Content = "Dette er post 1 i Blog 3",
                            Rating = 0,
                            Title = "Post 1"
                        });
                });

            modelBuilder.Entity("SavingData.Models.PostTag", b =>
                {
                    b.Property<int>("PostTagId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PostId");

                    b.Property<string>("TagId");

                    b.HasKey("PostTagId");

                    b.HasIndex("PostId");

                    b.HasIndex("TagId");

                    b.ToTable("PostTag");

                    b.HasData(
                        new
                        {
                            PostTagId = 1,
                            PostId = 1,
                            TagId = "Sport"
                        },
                        new
                        {
                            PostTagId = 2,
                            PostId = 2,
                            TagId = "Sport"
                        },
                        new
                        {
                            PostTagId = 3,
                            PostId = 2,
                            TagId = "News"
                        },
                        new
                        {
                            PostTagId = 4,
                            PostId = 3,
                            TagId = "News"
                        },
                        new
                        {
                            PostTagId = 5,
                            PostId = 4,
                            TagId = "Living"
                        },
                        new
                        {
                            PostTagId = 6,
                            PostId = 5,
                            TagId = "Photo"
                        },
                        new
                        {
                            PostTagId = 7,
                            PostId = 6,
                            TagId = "News"
                        });
                });

            modelBuilder.Entity("SavingData.Models.Tag", b =>
                {
                    b.Property<string>("TagId")
                        .ValueGeneratedOnAdd();

                    b.HasKey("TagId");

                    b.ToTable("Tag");

                    b.HasData(
                        new
                        {
                            TagId = "Photo"
                        },
                        new
                        {
                            TagId = "Sport"
                        },
                        new
                        {
                            TagId = "News"
                        },
                        new
                        {
                            TagId = "Money"
                        },
                        new
                        {
                            TagId = "Living"
                        },
                        new
                        {
                            TagId = "Children"
                        });
                });

            modelBuilder.Entity("SavingData.Models.Blog", b =>
                {
                    b.HasOne("SavingData.Models.Person", "Owner")
                        .WithMany("OwnedBlogs")
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("SavingData.Models.PersonPhoto", b =>
                {
                    b.HasOne("SavingData.Models.Person", "Person")
                        .WithOne("Photo")
                        .HasForeignKey("SavingData.Models.PersonPhoto", "PersonId");
                });

            modelBuilder.Entity("SavingData.Models.Post", b =>
                {
                    b.HasOne("SavingData.Models.Person", "Author")
                        .WithMany("AuthoredPosts")
                        .HasForeignKey("AuthorId");

                    b.HasOne("SavingData.Models.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SavingData.Models.PostTag", b =>
                {
                    b.HasOne("SavingData.Models.Post", "Post")
                        .WithMany("Tags")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SavingData.Models.Tag", "Tag")
                        .WithMany("Posts")
                        .HasForeignKey("TagId");
                });
#pragma warning restore 612, 618
        }
    }
}
