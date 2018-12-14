﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicTacToeServer.Database;

namespace TicTacToeServer.Migrations
{
    [DbContext(typeof(Db))]
    partial class DbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TicTacToeServer.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CurrentPlayerId");

                    b.Property<int>("FieldId");

                    b.HasKey("Id");

                    b.HasIndex("FieldId");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("TicTacToeServer.Models.GameField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Down");

                    b.Property<string>("DownLeft");

                    b.Property<string>("DownRight");

                    b.Property<string>("Middle");

                    b.Property<string>("MiddleLeft");

                    b.Property<string>("MiddleRight");

                    b.Property<string>("Top");

                    b.Property<string>("TopLeft");

                    b.Property<string>("TopRight");

                    b.HasKey("Id");

                    b.ToTable("GameField");
                });

            modelBuilder.Entity("TicTacToeServer.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("GameId");

                    b.Property<string>("GuestId");

                    b.Property<string>("GuestNick");

                    b.Property<string>("HostId");

                    b.Property<string>("HostNick")
                        .IsRequired();

                    b.Property<DateTime>("LastStateChangeDate");

                    b.Property<int>("NumberOfPlayersInside");

                    b.Property<string>("Password");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("TicTacToeServer.Models.Game", b =>
                {
                    b.HasOne("TicTacToeServer.Models.GameField", "Field")
                        .WithMany()
                        .HasForeignKey("FieldId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TicTacToeServer.Models.Room", b =>
                {
                    b.HasOne("TicTacToeServer.Models.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");
                });
#pragma warning restore 612, 618
        }
    }
}
