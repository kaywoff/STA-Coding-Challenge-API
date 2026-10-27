@auth
Feature: Authentication
  As a registered user
  I want to log in and see who I am logged in as
  So that I can use the parts of the API that need a token

  Scenario: Log in and keep the access token
    Given I have valid login credentials
    When I log in
    Then the response status code is 200
    And an access token is returned

  Scenario: Check the details of the logged-in user
    Given I am logged in as a valid user
    When I ask for the current user's details
    Then the response status code is 200
    And the logged-in user's id is 1
    And the logged-in user's first name is "Emily"
    And the logged-in user's last name is "Johnson"
