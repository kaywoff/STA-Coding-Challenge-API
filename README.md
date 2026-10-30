# DummyJSON API tests

API test automation for the [DummyJSON](https://dummyjson.com/docs) public REST API, written in C# with Gherkin scenarios.
There are nine scenarios, one for each exercise in the Senior Test Analyst coding challenge.

## Running the tests

I used the [.NET 10 SDK]

```bash
dotnet test
```
For a more detailed console log (each request and response status is written to the test output):
```bash
dotnet test --logger "console;verbosity=detailed"
```
The API address and the demo login live in `appsettings.json', so nothing is hard-coded in the steps.

## What is covered
| # | Area | Scenario | How it is checked |
| 1 | Auth | Log in and keep the token | 200 status, access token returned
| 2 | Auth | Current user |'GET /auth/me' with the Bearer token; id, first name, last name
| 3 | Users | Red hair count | Filter endpoint; every user is red-haired; total matches my own count of the full list
| 4 | Users | Add a user | 'POST /users/add'with name, age, gender, hair colour and type, and the full address including coordinates; the response repeats everything I sent.
| 5 | Products | iPhone 13 Pro price | Search; exactly one title match; price; same title and price on the product's own page.
| 6 | Products | 15th product | 'limit=1&skip=14&select=id,title,price'; exactly one product, only those three fields, id 15, matches the product's own page.
| 7 | Products | Update product 121 | 'PUT /products/121'; the response holds the new title.
| 8 | Cart | Add 5 products | Quantities, totals and discount percentage (see below).
| 9 | Cart | Delete a cart | 'DELETE /carts/1'; 'isDeleted' is true and 'deletedOn` is returned.

## How the project is laid out
Features/    the Gherkin scenarios, readable without looking at any code
Steps/       step definitions, kept short, one file per area
Clients/     ApiClient (the only code that does HTTP) plus one small client per resource
Models/      typed request and response objects
Support/     ScenarioState, which carries data between the steps of one scenario
Config/      reads appsettings.json
```

## Decisions I made, and why

**Reqnroll with xUnit.** Reqnroll is the activey maintaind continuation of SpecFlow, which is no longer being developed.
xUnit runs scenarios in parallel by default, so I kept every scenario independent of the others.

**HttpClient, no extra HTTP library.** The API is simple enough that 'HttpClient' and 'System.Text.Json' helped in getting the task done.
One shared 'HttpClient' is reused for the task. All HTTP lives in the 'AppClient', so if the API changesor when we swap the
library only one file is affected.

**Thin steps, typed models.** Steps read like the feature files and call a client. Responses are turned into C# classes
so assertions are on real properties instead of fragile string searches.

**Scenarios do not depend on each other.** The "current user" scenario logs in for itself rather than relying on the
first scenario having run, so any scenario can be run alone or in any order so it doesnt disturbthe code.

**DummyJSON does not save anything.** Posts, puts and deletes only pretend to work. So for add, update and delete I
only check the response to that request that isstatus code and body and never fetch the record afterwards expecting a change.

**A second opinion instead of magic numbers.** Where a value could change as the dataset changes, the test compares
two sources rather than hardcoding a number, scenario was as follows
- red hair count: the filter endpoint total versus my own count of the full user list
- 15th product and iPhone 13 Pro:  searched result versus the same product fetched by id.
The values that are written into the feature files (Emily Johnson with id 1, the iPhone price of 1099.99, product ids)
are the documented values for the demo user and data. If DummyJSON ever changes them, they are in one obvious place.

**The cart discount percentage.** The cart response gives the discount as money 'total' and 'discountedTotal' and gives a
`discountPercentage'per product, but no percentage for the whole cart. I calculate it two ways and require them to agree:
1. from the cart totals: 'total - discountedTotal/ total 100'
2. from the productseach product's discount percentage, weighted by what that line costs.
I also check that each line's discounted total follows its own percentage, and that the line totals add up to the cart
total. The API rounds to the penny, so the comparisons allow a small tolerance (2p on money, 0.1 percentage points on
percentages). This is my reading of "total discounted percentage".

**The brief says "Delete a Card". I took that to mean a cart, as it is under the Cart heading. I sent an email to verify but didn't get a response.

## Things I would add with more time

- Negative tests (wrong password, missing or expired token, unknown product)
- JSON schema checks on responses
- A readable HTML report (for example Allure or Reqnroll LivingDoc)
- Retry on network blips, and a smoke tag to run a quick subset

## Use of AI
I used an AI assistant to check errors while running the tests and kept on encountring these errors (error CS0111: Type 'CartSteps' already defines a member called 'ThenEachLineFollowsItsDiscount' with the same parameter types, /Users/kaywoffington/Documents/QA TASK/STA Coding Challenge – API  /STA Coding Challenge – API  /Steps/CartSteps.cs(95,64): error CS1061: 'CartLine' does not contain a definition for 'DiscountedTotal' and no accessible extension method 'DiscountedTotal' accepting a first argument of type 'CartLine' could be found (are you missing a using directive or an assembly reference?, DummyJson.ApiTests net10.0 failed with 1 error(s) (2.2s).
# Problems i ran into and how i fix them
I ran the suite against the live API as I went. These are the real problems I hit, in the order they happened.

1 `dotnet test'stopped straight away with 'TESTRUNABORT Test Run Aborted'and no scenarios ran 
 The project targeted .NET 8, but my machine only had the .NET 10 runtime, so the test host could not start so Changed the target framework to 'net10.0' and updated the GitHub Actions workflow to '10.0.x'so CI matches my machine.

2 The cart scenario failed: "cart discounted total 40814 should be the sum of its lines"
 My first check allowed only 2p of difference, but the API rounds discounted amounts to whole numbers | Added a separate, larger tolerance for rounded discounted amounts.

3The same step still failed, and the message showed the sum of the lines as '0'A sum of 0 meant my model was not reading that field at all. Calling the API directly with 'curl'showed that each cart line uses 'discountedPrice' while the cart itself uses 'discountedTotal s0 renamed the property on the cart line model to 'DiscountedPrice  and kept 'DiscountedTotal' on the cart.

4Build error 'CS1061: 'CartLine' does not contain a definition for 'DiscountedTotal' After renaming the property, one place in the steps still used the old name Updated the remaining references

5Build error 'CS0111: Type 'CartSteps' already defines a member called ..' While patching by hand I ended up with the same method twice s0 Replaced the whole file with a clean version instead of patching it line by line
6My 'curl'test returned User id is required and `zsh: command not found: -d' A blank line in the middle of a multi-line command ended it early  Re-ran it as one continuous command.

## Design challenges
Stateless API. DummyJSON pretends to save changes but does not, so for add, update and delete I only check the response to that request and never expect the change to show up in a later request, "Total discount percentage".The cart response gives the discount as money, plus a percentage per product, but no percentage for the whole cart. I work it out two w(from the cart totals, and from the per-product percentages weighted by line cost) and require them to agree, with a tolerance for rounding.
"Only this product" for pagination. I used limit=1, 'skip=14' and `select=id,title,price', then checked that the response contains one product with only those three fields.
-Nested address in Gherkin.
A flat value table in the scenario keeps it readable, and the step builds the nested address and coordinates from it Values that can change. Instead of hard coding numbers that depend on the dataset, I compare two sources, for example the red-hair filter total against my own count of the full user list.