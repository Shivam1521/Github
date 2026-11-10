import { authHeader, handleResponse } from "../helper/auth-header";

export default class ItemMasterService {
  async GetItems(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/Item/GetItemMasterByUserId`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
  async UpdateItem(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    

    const url = `${import.meta.env.VITE_API_URL}/Item/UpdateItemMaster`;
      console.log("========== UPDATE ITEM API REQUEST ==========");
  console.log("URL:", url);
  console.log("Method:", requestOptions.method);
  console.log("Request Body:", obj);
  console.log("Request JSON:", requestOptions.body);

    const response = await fetch(url, requestOptions);

  // RESPONSE LOG
  console.log("========== UPDATE ITEM API RESPONSE ==========");
  console.log("Status:", response.status);
  console.log("OK:", response.ok);
    const data = await handleResponse(response, { url, ...requestOptions });
     console.log("Response Body:", data);
    return data;
  }
  async UploadItemMapping(file) {
  const formData = new FormData();
  formData.append("File", file);

  const requestOptions = {
    method: "POST",
    headers: {
      Authorization: authHeader().Authorization,
    },
    body: formData,
  };

  const url = `${import.meta.env.VITE_API_URL}/Item/MapItemsAndUsers`;

  const response = await fetch(url, requestOptions);
  const data = await handleResponse(response, { url, ...requestOptions });

  return data;
}
}

