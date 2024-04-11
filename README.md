## Steps:

1. **Generate OTP:**
    - **Endpoint:** `/api/authentication`
    - **Description:** This endpoint generates an OTP, saves it to the database associated with the phone number, and sends the OTP to the client.
    - **Request Body:** JSON
      ```json
      {
         "phone": "+123456"
      }
      ```
    - **Expected Response:** Status code 200 if successful.

2. **Verify OTP:**
    - **Endpoint:** `/api/authentication/verify`
    - **Description:** This endpoint verifies a pair of phone number and OTP against the database. If the OTP is associated with the phone number in the database, a JWT token is generated.
    - **Request Body:** JSON
      ```json
      {
          "phone": "+123456",
          "otp": 123456
      }
      ```
    - **Expected Response:**
        - Status code 200 and a JWT token with claim containing `cardGuid` if successful.
        - Error if there is no OTP associated with the provided phone number.

3. **Example Usage with JWT Token:**
    - **Endpoint:** `/api/user`
    - **Description:** This endpoint demonstrates the use of JWT token for authentication.
    - **Authorization:**
        - Click "Authorize" button in Swagger.
        - Paste the JWT token obtained from the response of step 2 into the "Bearer (http, Bearer)" value field.

## How to Test:

1. Start by generating an OTP using the `/api/authentication` endpoint.
2. Verify the OTP using the `/api/authentication/verify` endpoint.
3. Use the generated JWT token to access protected endpoints, such as `/api/user`.

## Documentation:

- Access the Swagger documentation [here](http://localhost:5096/swagger/index.html).