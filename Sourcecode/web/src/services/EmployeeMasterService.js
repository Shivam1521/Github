import { authHeader, handleResponse } from "../helper/auth-header";

export default class EmployeeMasterService {
  async GetUsers(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/User/GetUsers`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }

  async CreateUpdateUser(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/User/CreateUpdateUser`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
