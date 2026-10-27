@users
Feature: Users
  As a consumer of the users API
  I want to find and add users
  So that I know the user endpoints behave as documented

  Scenario: Count the users with red hair
    When I search for users with "Red" hair colour
    Then the response status code is 200
    And every user returned has "Red" hair
    And the total agrees with a manual count of "Red" hair in the full user list

  Scenario: Add a new user
    Given I have a new user with these details
      | field      | value          |
      | firstName  | Amelia         |
      | lastName   | Hartley        |
      | age        | 29             |
      | gender     | female         |
      | hairColour | Auburn         |
      | hairType   | Wavy           |
      | street     | 14 Mill Lane   |
      | city       | Manchester     |
      | state      | England        |
      | stateCode  | ENG            |
      | postalCode | M1 4BT         |
      | latitude   | 53.4808        |
      | longitude  | -2.2426        |
      | country    | United Kingdom |
    When I add the new user
    Then the response status code is 201
    And the new user has been given an id
    And the response repeats every detail I sent
