# DummyJSON API tests

API test automation for the [DummyJSON](https://dummyjson.com/docs) public REST API, written in C# with Gherkin scenarios.
There are nine scenarios, one for each exercise in the Senior Test Analyst coding challenge.

## Running the tests

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download) and an internet connection.

```bash
dotnet test
```

For a more detailed console log (each request and response status is written to the test output):

```bash
dotnet test --logger "console;verbosity=detailed"
```

The API address and the demo login live in `appsettings.json`, so nothing is hard-coded in the steps.

## What is covered

| # | Area | Scenario | How it is checked |
|---|------|----------|-------------------|
| 1 | Auth | Log in and keep the token | 200 status, access token returned |
| 2 | Auth | Current user | `GET /auth/me` with the Bearer token; id, first name, last name |
| 3 | Users | Red hair count | Filter endpoint; every user is red-haired; total matches my own count of the full list |
| 4 | Users | Add a user | `POST /users/add` with name, age, gender, hair colour and type, and the full address including coordinates; the response repeats everything I sent |
| 5 | Products | iPhone 13 Pro price | Search; exactly one title match; price; same title and price on the product's own page |
| 6 | Products | 15th product | `limit=1&skip=14&select=id,title,price`; exactly one product, only those three fields, id 15, matches the product's own page |
| 7 | Products | Update product 121 | `PUT /products/121`; the response holds the new title |
| 8 | Cart | Add 5 products | Quantities, totals and discount percentage (see below) |
| 9 | Cart | Delete a cart | `DELETE /carts/1`; `isDeleted` is true and `deletedOn` is returned |

## How the project is laid out

```
Features/    the Gherkin scenarios, readable without looking at any code
Steps/       step definitions, kept short, one file per area
Clients/     ApiClient (the only code that does HTTP) plus one small client per resource
Models/      typed request and response objects
Support/     ScenarioState, which carries data between the steps of one scenario
Config/      reads appsettings.json
```

## Decisions I made, and why

**Reqnroll with xUnit.** Reqnroll is the actively maintained continuation of SpecFlow, which is no longer being developed.
xUnit runs scenarios in parallel by default, so I kept every scenario independent of the others.

**HttpClient, no extra HTTP library.** The API is simple enough that `HttpClient` plus `System.Text.Json` does the job.
One shared `HttpClient` is reused for the whole run. All HTTP lives in `ApiClient`, so if the API changes (or I swap the
library) only one file is affected.

**Thin steps, typed models.** Steps read like the feature files and call a client. Responses are turned into C# classes
so assertions are on real properties instead of fragile string searches.

**Scenarios do not depend on each other.** The "current user" scenario logs in for itself rather than relying on the
first scenario having run, so any scenario can be run alone or in any order.

**DummyJSON does not save anything.** Posts, puts and deletes only pretend to work. So for add, update and delete I
only check the response to that request (status code and body) and never fetch the record afterwards expecting a change.

**A second opinion instead of magic numbers.** Where a value could change as the dataset changes, the test compares
two sources rather than hard-coding a number:
- red hair count: the filter endpoint total versus my own count of the full user list;
- 15th product and iPhone 13 Pro: the paginated or searched result versus the same product fetched by id.
The values that are written into the feature files (Emily Johnson with id 1, the iPhone price of 1099.99, product ids)
are the documented values for the demo user and data. If DummyJSON ever changes them, they are in one obvious place.

**The cart discount percentage.** The cart response gives the discount as money (`total` and `discountedTotal`) and gives a
`discountPercentage` per product, but no percentage for the whole cart. I calculate it two ways and require them to agree:
1. from the cart totals: `(total - discountedTotal) / total * 100`;
2. from the products: each product's discount percentage, weighted by what that line costs.
I also check that each line's discounted total follows its own percentage, and that the line totals add up to the cart
total. The API rounds to the penny, so the comparisons allow a small tolerance (2p on money, 0.1 percentage points on
percentages). This is my reading of "total discounted percentage"; if the intention was different it is a single step to change.

**The brief says "Delete a Card".** I took that to mean a cart, as it is under the Cart heading.

## Things I would add with more time

- Negative tests (wrong password, missing or expired token, unknown product)
- JSON schema checks on responses
- A readable HTML report (for example Allure or Reqnroll LivingDoc)
- Retry on network blips, and a smoke tag to run a quick subset

## Use of AI

> **Edit this section so it describes what you really did, because the challenge asks for an honest disclaimer.**

I used an AI assistant (Claude, by Anthropic) to help plan this solution and to draft the code, the feature files and
this README. I reviewed the code, ran the suite, and I understand how each part works and can explain any of the design
choices above.
# dummyjson-api-tests
