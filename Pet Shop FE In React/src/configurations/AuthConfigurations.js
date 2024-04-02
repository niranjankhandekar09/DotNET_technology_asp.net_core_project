const ParentConfiguration = require("./ParentConfiguration");

module.exports = {
  SignUp: ParentConfiguration.Authentication + "api/Auth/SignUp",
  SignIn: ParentConfiguration.Authentication + "api/Auth/SignIn",
  // SignUp: "https://localhost:44381/api/Auth/SignUp",
  // SignIn: "https://localhost:44381/api/Auth/SignIn",
  AddCustomerDetail:
    ParentConfiguration.Authentication + "api/Auth/AddCustomerDetail",
  CustomerList: ParentConfiguration.Authentication + "api/Auth/CustomerList",
  AddCustomerAdderess:
    ParentConfiguration.Authentication + "api/Auth/AddCustomerAdderess",
  GetCustomerAdderess:
    ParentConfiguration.Authentication +
    "api/Auth/GetCustomerAdderess/?UserID=",
  GetCustomerDetail:
    ParentConfiguration.Authentication + "api/Auth/GetCustomerDetail?UserID=",
  GetIsCustomerDetailsFound:
    ParentConfiguration.Authentication +
    "api/Auth/GetIsCustomerDetailsFound?UserID=",
};
