import { authHeader, handleResponse } from "../helper/auth-header";

export default class PickedSlipRptService {
  async fetchPickedSlipRpt(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/Pickslip/GetPickedslipReport`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
