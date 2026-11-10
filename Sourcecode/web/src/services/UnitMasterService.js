import { authHeader, handleResponse } from "../helper/auth-header";

export default class UnitMasterService {
  async GetUnits(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/Unit/GetUnits`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }

  async CreateUpdateUnit(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/Unit/CreateUpdateUnit`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
