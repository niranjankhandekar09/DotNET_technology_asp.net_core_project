import React, { Component } from "react";
import AuthServices from "../services/AuthServices";
import "./SignIn.css";
import TextField from "@material-ui/core/TextField";
import Radio from "@material-ui/core/Radio";
import RadioGroup from "@material-ui/core/RadioGroup";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Button from "@material-ui/core/Button";
import Snackbar from "@material-ui/core/Snackbar";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";

import Backdrop from "@material-ui/core/Backdrop";
import CircularProgress from "@material-ui/core/CircularProgress";

const authServices = new AuthServices();

export default class SignIn extends Component {
  constructor() {
    super();
    this.state = {
      Radiovalue: "Customer",
      UserName: "",
      UserNameFlag: false,
      Password: "",
      PasswordFlag: false,
      open: false,
      Message: "",
      OpenLoader: false,
    };
  }

  handleClose = (e, reason) => {
    if (reason === "clickaway") {
      return;
    }
    this.setState({ open: false });
  };

  handleRadioChange = (e) => {
    this.setState({ Radiovalue: e.target.value });
  };

  handleChange = (e) => {
    const { name, value } = e.target;
    this.setState(
      { [name]: value },
      console.log("Name : ", name, "Value : ", value)
    );
  };

  handleSignUp = (e) => {
    this.props.history.push("/");
  };

  CheckValidation() {
    console.log("CheckValidation Calling...");

    this.setState({ UserNameFlag: false, PasswordFlag: false });

    if (this.state.UserName === "") {
      this.setState({ UserNameFlag: true });
    }
    if (this.state.Password === "") {
      this.setState({ PasswordFlag: true });
    }
  }

  handleSubmit = (e) => {
    this.CheckValidation();
    if (this.state.UserName !== "" && this.state.Password !== "") {
      console.log("Acceptable");
      this.setState({ OpenLoader: true });
      let data = {
        userName: this.state.UserName,
        password: this.state.Password,
        role: this.state.Radiovalue,
      };
      authServices
        .SignIn(data)
        .then((data) => {
          console.log("Sign In Data : ", data);
          this.setState({ OpenLoader: false });
          if (data.data.isSuccess) {
            //
            if (this.state.Radiovalue === "Customer") {
              localStorage.setItem("customer_token", data.data.data.token);
              localStorage.setItem("Customer_UserID", data.data.data.userId);
              localStorage.setItem("Customer_UserName", data.data.data.userName);
              localStorage.setItem("OpenUserHome", true);
              localStorage.setItem("OpenMyOrder", false);
              localStorage.setItem("OpenCard", false);
              localStorage.setItem("OpenWishList", false);
              localStorage.setItem("OpenCustomerSetting", false);
              localStorage.setItem("OpenUserDetail", false);
              localStorage.setItem("OpenUserAddress", false);
              this.props.history.push("/UserDashboard");
            } else {
              localStorage.setItem("Admin_token", data.data.data.token);
              localStorage.setItem("Admin_UserID", data.data.data.userId);
              localStorage.setItem("OpenHome", true);
              localStorage.setItem("OpenAddProduct", false);
              localStorage.setItem("OpenArchive", false);
              localStorage.setItem("OpenTrash", false);
              localStorage.setItem("OpenCustomerList", false);
              localStorage.setItem("OpenOrderList", false);
              localStorage.setItem("OpenFeedBack", false);
              this.props.history.push("/AdminDashboard");
            }
          } else {
            console.log("Something Went Wrong");
            this.setState({ open: true, Message: data.data.message });
          }
        })
        .catch((error) => {
          console.log("Sign In Error : ", error);
          this.setState({OpenLoader: false, open: true, Message: "Something Went Wrong" });
        });
    } else {
      console.log("Not Acceptable");
      this.setState({ open: true, Message: "Please Field Mandetory Field" });
    }
  };

  render() {
    console.log("State : ", this.state);
    return (
      <div className="SignIn-Container">
        <div className="SignUp-SubContainer">
          <div className="Title">Pet Shop</div>
          <div className="Header_Container">Log In</div>
          <div className="Body">
            <form className="form">
              <TextField
                className="TextField"
                name="UserName"
                label="UserName"
                variant="outlined"
                size="small"
                style={{ margin: 20 }}
                error={this.state.UserNameFlag}
                value={this.state.UserName}
                onChange={this.handleChange}
              />
              <TextField
                className="TextField"
                type="password"
                name="Password"
                label="Password"
                variant="outlined"
                size="small"
                style={{ margin: 20 }}
                error={this.state.PasswordFlag}
                value={this.state.Password}
                onChange={this.handleChange}
              />
              <RadioGroup
                className="Roles"
                name="Role"
                value={this.state.Radiovalue}
                onChange={this.handleRadioChange}
              >
                <FormControlLabel
                  className="RoleValue"
                  value="Admin"
                  control={<Radio />}
                  label="Admin"
                />
                <FormControlLabel
                  className="RoleValue"
                  value="Customer"
                  control={<Radio />}
                  label="Customer"
                />
              </RadioGroup>
            </form>
          </div>
          <div className="Buttons" style={{ alignItems: "flex-start" }}>
            <Button className="Btn" color="primary" onClick={this.handleSignUp}>
              Create New Account
            </Button>
            <Button
              className="Btn"
              variant="contained"
              color="primary"
              onClick={this.handleSubmit}
            >
              Sign In
            </Button>
          </div>
        </div>
        <Snackbar
          anchorOrigin={{
            vertical: "bottom",
            horizontal: "left",
          }}
          open={this.state.open}
          autoHideDuration={6000}
          onClose={this.handleClose}
          message={this.state.Message}
          action={
            <React.Fragment>
              <Button color="secondary" size="small" onClick={this.handleClose}>
                UNDO
              </Button>
              <IconButton
                size="small"
                aria-label="close"
                color="inherit"
                onClick={this.handleClose}
              >
                <CloseIcon fontSize="small" />
              </IconButton>
            </React.Fragment>
          }
        />
        <Backdrop
          style={{ zIndex: "1", color: "#fff" }}
          open={this.state.OpenLoader}
        >
          <CircularProgress color="inherit" />
        </Backdrop>
      </div>
    );
  }
}
