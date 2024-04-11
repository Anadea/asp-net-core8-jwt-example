Steps:
1. /api/authentication - generate OTP, save it to DB associated with phone, send OTP to client.
2. /api/authentication/verify -  verify a pair, phone, and OTP against the database. If there is an OTP associated with phone in DB, generate a JWT token. It should have claim with cardGuid. If there isn't an OTP associated with phone in DB, returns error.
3. /api/user - this is an example how to use JWT token. 
