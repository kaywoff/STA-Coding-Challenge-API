@cart
Feature: Cart
  As a shopper
  I want to fill a cart and remove it again
  So that I know the cart totals and discounts are worked out correctly

  Scenario: Add five products to a cart and check the discount percentage
    Given I am shopping as user 1
    When I add these products to a cart
      | productId | quantity |
      | 1         | 2        |
      | 15        | 1        |
      | 98        | 3        |
      | 144       | 4        |
      | 150       | 1        |
    Then the response status code is 201
    And the cart contains 5 different products
    And the quantities in the cart match what I asked for
    And the cart totals add up
    And each product's discounted total follows its discount percentage
    And the overall discount percentage is consistent with the discounts on each product

  Scenario: Delete a cart
    When I delete cart 1
    Then the response status code is 200
    And the deleted cart is cart 1
    And the cart is marked as deleted
    And a deletion date is returned
