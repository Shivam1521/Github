import { authHeader, handleResponse } from "../helper/auth-header";

export default class PrintingService {
  async SerialUploadExcel(formData) {
    const headers = authHeader();
    // remove content-type if it exists, let browser set it for FormData
    if (headers["Content-Type"]) {
      delete headers["Content-Type"];
    }
    const requestOptions = {
      method: "POST",
      headers,
      body: formData,
    };

    const url = `${import.meta.env.VITE_API_URL}/SerialUpload/UploadSerialExcel`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
