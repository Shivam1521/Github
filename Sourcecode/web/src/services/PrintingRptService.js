import { authHeader, handleResponse } from "../helper/auth-header";

export default class PrintingRptService {
  async fetchPrintingData(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/Report/GetSerialUploadReport`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
  async fetchJobData(srNo) {
    const requestOptions = {
      method: "GET",
      headers: authHeader(),
    };

    const url = `${import.meta.env.VITE_API_URL}/Report/GetPrintJobReport?Srno=${srNo}`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
