import { authHeader, handleResponse } from "../helper/auth-header";

export default class PickSlipRptService {
  async fetchPickSlipData(pickSlipNumber) {
    const requestOptions = {
      method: "GET",
      headers: authHeader(),
    };

    const url = `${import.meta.env.VITE_API_URL}/Pickslip/GetPickSlipReceiptStatus?pickslipNumber=${pickSlipNumber}`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
