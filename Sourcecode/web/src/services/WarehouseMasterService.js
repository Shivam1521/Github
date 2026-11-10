import { authHeader, handleResponse } from "../helper/auth-header";

export default class WarehouseMasterService {
  async GetPlantWarehouses(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/PlantWarehouse/GetPlantWarehouses`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }

  async CreateUpdatePlantWarehouse(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/PlantWarehouse/CreateUpdatePlantWarehouse`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
