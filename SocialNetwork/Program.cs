using Microsoft.EntityFrameworkCore;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Services;
using SocialNetwork.DTOs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SocialNetworkContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=SocialNetwork;Username=postgres;Password=1234",
        b => b.MigrationsAssembly("SocialNetwork"));
});

//dotnet ef migrations add init
//dotnet ef database update

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("api/adduser", async (UserInputDto inputUser,IUserService userService) =>
{
    var newUser = await userService.AddUser(inputUser.FirstName, inputUser.LastName, inputUser.Birthday);
    return Results.Created($"api/adduser/{newUser.Id}", newUser);
});

app.MapPost("api/publications", async (PublicationInputDto publication, IUserService userService) =>
{
    var newPublication = await userService.AddPublication(publication.TextContent, publication.MediaContent, publication.UserGuidId);

    return Results.Created($"api/publications/{newPublication.Id}",newPublication);
});

app.MapPost("api/getpublications", async (Guid curUserId, Guid userId, IUserService userService) =>
{
    var listPublication = await userService.GetListPublications(curUserId, userId);

    return Results.Created($"api/getpublications/", listPublication);
});

app.Run();

public class SocialNetworkContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Publication> Publications { get; set; }

    public SocialNetworkContext(DbContextOptions<SocialNetworkContext> options) : base(options)
    {
    }

    public SocialNetworkContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasKey(k => k.Id);

        modelBuilder.Entity<User>().ToTable(nameof(User));

        modelBuilder.Entity<User>().Property(r => r.Id).ValueGeneratedNever();
        modelBuilder.Entity<Publication>().Property(r => r.Id).ValueGeneratedNever();

        modelBuilder.Entity<User>().OwnsOne(x => x.UserName,
            a =>
            {
                a.Property(p => p.FirstName)
                    .HasColumnName(nameof(User.UserName.FirstName))
                    .HasMaxLength(50)
                    .IsRequired();
            });

        modelBuilder.Entity<User>().OwnsOne(x => x.UserName,
            a =>
            {
                a.Property(p => p.LastName)
                    .HasColumnName(nameof(User.UserName.LastName))
                    .HasMaxLength(50)
                    .IsRequired();
            });

        modelBuilder.Entity<User>().OwnsOne(x => x.Birthday,
            a =>
            {
                a.Property(p => p.Date)
                    .HasColumnName(nameof(User.Birthday))
                    .IsRequired();
            });

        modelBuilder.Entity<User>()
            .HasMany(user => user.Publications)
            .WithOne(publication => publication.User);

        modelBuilder.Entity<UsersFriends>(entity =>
        {
            entity.HasKey(source => new { source.UserId, source.UsersFriendId });
        });

    }
}