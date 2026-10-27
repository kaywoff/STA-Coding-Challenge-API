@products
Feature: Products
  As a shopper
  I want to look products up, page through them and update them
  So that I can trust the product data the API gives me

  Scenario: Check the price of the iPhone 13 Pro
    When I search for the product "iPhone 13 Pro"
    Then the response status code is 200
    And the search results include a product titled "iPhone 13 Pro"
    And the product costs 1099.99
    And the product's own page shows the same title and price

  Scenario: Get the 15th product using pagination
    When I request product number 15 with a page size of 1
    Then the response status code is 200
    And the response contains exactly 1 product
    And the product contains only the id, title and price
    And the product id is 15
    And the product's own page shows the same title and price

  Scenario: Update the title of product 121
    When I change the title of product 121 to "Hand-picked by the test suite"
    Then the response status code is 200
    And the product id is 121
    And the product title is "Hand-picked by the test suite"
