# SportradarSkillsTest

This is my solution to the exercise for applying to the position of Senior .Net Developer.
The exercise statement is [here](https://github.com/ibanrg/SportradarSkillsTest/blob/master/SportradarSkillsTest/Docs/2025%20Sportradar%20NET%20Test%20Exercise.pdf)

### Some considerations to keep in mind:

- Bets that arrive as OPEN are counted toward the total bet amount.
- No more than two bets with the same Id are expected to arrive.
- If a bet arrives with an incorrect state sequence, it is marked for review, along with all subsequent bets that arrive with the same Id.
- Bets that end with the VOID status are not considered, as we are not managing the customer's balance. They are not included in the Profit/Loss calculation because their value is 0.

### API calls:

POST /bets/initialize --> Add 100 bets to system from file [bets.json](https://github.com/ibanrg/SportradarSkillsTest/blob/master/SportradarSkillsTest/Data/bets.json)\
POST /bets -->  Adds a given bet to the system\
GET /bets/summary --> Returns a summary of currently processed bets\
GET /bets/review --> Returns a list of bets marked for review\
POST /bets/shutdown --> Gracefully completes processing of queued bets and then shuts down the system\

I have attached a Postman collection with the possible API calls here: [Postman Collection](
https://github.com/ibanrg/SportradarSkillsTest/blob/master/SportradarSkillsTest/Docs/SportradarSkillsTest.postman_collection.json)

### Test Coverage

Test coverage is 90% line-rate and branch-rate. Couldn´t reach 95% due to conditions based on private fields.
I have attached report here: [Test Coverage Report](https://github.com/ibanrg/SportradarSkillsTest/blob/master/SportradarSkillsTest/Docs/test-coverage.xml)

