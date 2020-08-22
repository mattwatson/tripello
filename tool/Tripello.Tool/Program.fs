open Tripello.Server.Web.Data
open Microsoft.EntityFrameworkCore;

let upgradeDatabase (connectionString : string) =

    let optionsBuilder =
        DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connectionString)
    use dbContext = new ApplicationDbContext(optionsBuilder.Options)
    dbContext.Database.Migrate()

[<EntryPoint>]
let main argv =
    match argv with
    | [| "updateDatabase"; connectionString |] -> upgradeDatabase connectionString
    | args -> failwithf "Unknown args: %A" args

    0
