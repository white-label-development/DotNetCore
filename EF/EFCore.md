# EF

## EF Core Getting Started Notes

### 4.5

visualizor - same proj dbcontext
needs DGML editor in VS (indiv components, code tools)
`<TargetFrameworks>netcoreapp3.0, netstandard2.0`
and add MS.EFC.Design

### 5

```c#
ctx.Samuarais.Add(samurai); // add samurai to samurais
ctx.AddRange(samurai, clan); // add 2 unrelated records together
```

### 5.3

MS.Extensions.Logging
MS.Extensions.Logging.Console ... see asp.core section
where(s => EF.Functions.Like(s.name, "j$")

disconnected clients:

`ctx.Battles.Update(battle); // ef does not know what prop has changed so wil send all`

`DbSet.AsTracking(); // to enable dbcontext no tracking for specific query`

### 6

```c#
newCtx.Samurais.Attach(samuariWithNewQuote); // samurai is tagged as unmodified but new quote (no id) is added
// .Update() sets as added (no key) or modified (has key)

//BUT if you expose FK the cleanest way to add a Samurai quote is to manually set newQuote.SamuraiId = x then
newCtx.Quotes.Add(quote);

EagerLoading = .Include .ThenInclude (no filters)

Query Projections = .Select(s => s.new {s.Id, s.Name,
s.Quotes.Where(q => q.Text.Contains("happy")).Count}

Explicit loading
// with samurai object already in memory
_context.Entry(mySamurai).Collection(s => s.Quotes).Where(..whatever..).Load();
_context.Entry(mySamurai).Reference(s => s.Horse).Load(); // 1 horse

// entry is specific to the object passed in
_context.Entry(editedQuote).State = EntityState.Modified

var sbJoin = new SamuraiBattle {SamuraiId = 1, Battle = 3 };
_context.Add(sbJoin); // as we don't need to expose a dbset for SamuraiBattle

//many to manty queries
//can do
_ctx.Samurais.Include(s => s.SamuraiBattles).ThenInclude(sb => sb.Battle)

//cleaner to project
_ctx.Samuaris.Select(s => new {
 Sumarai = s, Battles = s.SamuraiBattles.Select(sb => sb.Battle)
})

// EF Core 2 Mappings course  worth a look.

_ctx.Set<Horse>.Find(3); //qry a set without a defined dbSet (might be a navigation entity only)
```

### 7

`modelBuilder.Entity<SamuariBattleStat>().HasNoKey().ToView("view_Stats"); // will be read only then. Does not have to be a view, can be a table`

`ctx.Samurais.FromSQLRaw, ctx.Samurais.FromSQLInterpolated($"xxx {y}", foo)`

RawSQL: must return data for all properties of the entity type, no related data, only query entities known by DbContext

Interpolate will produce parameterized queries (preferred)

`_ctx.Database.ExecuteSqlRaw //not for queries. returns changed rows count only (or -1).`

### 8 ASP.NET

vscode rest client worth a look. lighter than Postman.

in controller put, note use of

```c#
_context.Entry(samaurai).State = EntityState.Modified;
//to ensure only the head of the graph (the samurai) gets updated. connected classes will not.
```

### 9 testing ef core

```c#
// test config vs live...
// in ctx onConfiguring
if(!optionsBuilder.IsConfigured) .. useSQLServer. //test uses other ctor DbContextOptions
```

## MS DOCS extact

```c#

modelBuilder.Ignore<BlogMetadata>(); // If you don't want a type to be included in the model, you can exclude it:

modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations());

modelBuilder.Entity<Blog>().ToTable("blogs");

modelBuilder.Entity<Blog>().ToView("blogsView", schema: "blogging");

modelBuilder.Entity<Blog>().Ignore(b => b.LoadedFromDatabase); // [NotMapped]

entitybuilder.Property(b => b.Rating).HasColumnType("decimal(5, 2)");

modelBuilder.Entity<Blog>().Property(b => b.Url).IsRequired();

modelBuilder.Entity<Car>().HasKey(c => c.LicensePlate); //  [Key]

modelBuilder.Entity<Car>().HasKey(c => new { c.State, c.LicensePlate });

// By convention, an alternate key is introduced for you when you identify a property which isn't the primary key as the target of a relationship.
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasOne(p => p.Blog)
        .WithMany(b => b.Posts)
        .HasForeignKey(p => p.BlogUrl)
        .HasPrincipalKey(b => b.Url); // otherwise would try to map the FK to the PrimaryKey on the principal
}

// You can also configure a single property to be an alternate key:
modelBuilder.Entity<Car>().HasAlternateKey(c => c.LicensePlate);

// composite
modelBuilder.Entity<Car>().HasAlternateKey(c => new { c.State, c.LicensePlate });

// You can configure any property to have its value generated for inserted entities as follows:
modelBuilder.Entity<Blog>().Property(b => b.Inserted).ValueGeneratedOnAdd();

modelBuilder.Entity<Blog>().Property(b => b.Rating).HasDefaultValue(3);

modelBuilder.Entity<Blog>().Property(b => b.Created).HasDefaultValueSql("getdate()");

modelBuilder.Entity<Blog>().Property(b => b.LastUpdated).ValueGeneratedOnAddOrUpdate();

modelBuilder.Entity<Person>().Property(p => p.DisplayName).HasComputedColumnSql("[LastName] + ', ' + [FirstName]");

modelBuilder.Entity<Person>().Property(p => p.NameLength).HasComputedColumnSql("LEN([LastName]) + LEN([FirstName])", stored: true);

modelBuilder.Entity<Blog>().Property(b => b.BlogId).ValueGeneratedNever();

 
// https://docs.microsoft.com/en-us/ef/core/saving/concurrency

modelBuilder.Entity<Person>().Property(p => p.LastName).IsConcurrencyToken();

modelBuilder.Entity<Blog>().Property(p => p.Timestamp).IsRowVersion(); // public byte[] Timestamp { get; set; }

// https://docs.microsoft.com/en-us/ef/core/modeling/shadow-properties

// To configure a relationship in the Fluent API, you start by
// identifying the navigation properties that make up the relationship.
// HasOne or HasMany identifies the navigation property on the entity type
// you are beginning the configuration on.
// You then chain a call to WithOne or WithMany to identify the inverse navigation.
// HasOne/WithOne are used for reference navigation properties and HasMany/WithMany are used for collection navigation properties.

modelBuilder.Entity<Post>().HasOne(p => p.Blog).WithMany(b => b.Posts); // includes inverse nav

modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(); // no nav from post to blog

//can futher configure the above ^^
modelBuilder.Entity<Blog>().Navigation(b => b.Posts).UsePropertyAccessMode(PropertyAccessMode.Property);

https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-composite-key%2Csimple-key

modelBuilder.Entity<Post>().HasOne(p => p.Blog)
            .WithMany(b => b.Posts)
            .HasForeignKey(p => p.BlogForeignKey); // as non conventional name

modelBuilder.Entity<Car>().HasKey(c => new { c.State, c.LicensePlate });

modelBuilder.Entity<RecordOfSale>()
            .HasOne(s => s.Car)
            .WithMany(c => c.SaleHistory)
            .HasForeignKey(s => new { s.CarState, s.CarLicensePlate });

// Add the shadow property to the model
modelBuilder.Entity<Post>().Property<int>("BlogForeignKey");

// Use the shadow property as a foreign key
modelBuilder.Entity<Post>().HasOne(p => p.Blog).WithMany(b => b.Posts).HasForeignKey("BlogForeignKey");

// If you want the foreign key to reference a property other than the primary key (alternate key)
modelBuilder.Entity<RecordOfSale>()
            .HasOne(s => s.Car)
            .WithMany(c => c.SaleHistory)
            .HasForeignKey(s => s.CarLicensePlate)
            .HasPrincipalKey(c => c.LicensePlate);
// links s.CarLicensePlate on RecordOfSale to LicensePlate on car, instead of PK


// the dependent entity is required to have a corresponding principal entity.
modelBuilder.Entity<Post>().HasOne(p => p.Blog).WithMany(b => b.Posts).IsRequired();

.OnDelete(DeleteBehavior.Cascade);

// in a one-to-one relationship make it clear which is the principal
modelBuilder.Entity<Blog>().HasOne(b => b.BlogImage).WithOne(i => i.Blog)
            .HasForeignKey<BlogImage>(b => b.BlogForeignKey);

// many to many. PostTags is the join table
modelBuilder
    .Entity<Post>()
    .HasMany(p => p.Tags)
    .WithMany(p => p.Posts)
    .UsingEntity(j => j.ToTable("PostTags"));

// You can also represent a many-to-many relationship by just adding the join entity type and mapping two separate one-to-many relationships
modelBuilder.Entity<PostTag>().HasKey(t => new { t.PostId, t.TagId });

modelBuilder.Entity<PostTag>()
    .HasOne(pt => pt.Post)
    .WithMany(p => p.PostTags)
    .HasForeignKey(pt => pt.PostId);

modelBuilder.Entity<PostTag>()
    .HasOne(pt => pt.Tag)
    .WithMany(t => t.PostTags)
    .HasForeignKey(pt => pt.TagId);

modelBuilder.Entity<Person>().HasIndex(p => new { p.FirstName, p.LastName });

// backing fields
modelBuilder.Entity<Blog>()
        .Property(b => b.Url)
        .HasField("_validatedUrl");


// data into field not public prop
modelBuilder.Entity<Blog>().Property("_validatedUrl");


// https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Modeling/OwnedEntities

modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress);

modelBuilder.Entity<Order>().OwnsOne(typeof(StreetAddress), "ShippingAddress"); // if private

// To configure a collection of owned types use OwnsMany
modelBuilder.Entity<Distributor>().OwnsMany(p => p.ShippingCenters, a =>
{
    a.WithOwner().HasForeignKey("OwnerId");

    a.Property<int>("Id");

    a.HasKey("Id");

});


// https://docs.microsoft.com/en-us/ef/core/modeling/keyless-entity-types?tabs=data-annotations
modelBuilder.Entity<BlogPostsCount>().HasNoKey(); // [Keyless]
