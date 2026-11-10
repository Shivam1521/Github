import { authHeaderNoToken, handleResponse } from "../helper/auth-header";

export default class PAService {
  async VerifyQR(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeaderNoToken(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/QRAuthentication/VerifyQR`;
    let data;
    try {
      const response = await fetch(url, requestOptions);
      data = await handleResponse(response, { url, ...requestOptions });
    } catch (error) {}
    return data;
  }
}
