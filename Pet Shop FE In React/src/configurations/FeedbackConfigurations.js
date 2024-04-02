const ParentConfiguration = require("./ParentConfiguration");

module.exports = {
  // GetFeedbacks: 'https://localhost:44381/api/Feedback/GetFeedbacks',
  // AddFeedback: 'https://localhost:44381/api/Feedback/AddFeedback',
  // DeleteFeedback: 'https://localhost:44381/api/Feedback/DeleteFeedback?ID=',
  GetFeedbacks: ParentConfiguration.Feedback + "api/Feedback/GetFeedbacks",
  AddFeedback: ParentConfiguration.Feedback + "api/Feedback/AddFeedback",
  DeleteFeedback: ParentConfiguration.Feedback + "api/Feedback/DeleteFeedback?ID=",
};
