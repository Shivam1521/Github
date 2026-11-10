import { authHeader, handleResponse } from "../helper/auth-header";

export default class CommonService {
  async GetList(listName) {
    const requestOptions = {
      method: "GET",
      headers: authHeader(),
    };

    const url = `${import.meta.env.VITE_API_URL}/List/GetList?listName=${listName}`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
  async GetRolesDDL(userType) {
    const requestOptions = {
      method: "GET",
      headers: authHeader(),
    };

    const url = `${import.meta.env.VITE_API_URL}/Role/GetRolesDDL?userType=${userType}`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
  async GetUnitsDDL() {
    const requestOptions = {
      method: "GET",
      headers: authHeader(),
    };

    const url = `${import.meta.env.VITE_API_URL}/Unit/GetUnitsDDL`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
  async GetPlantWarehouseDDL(unitId) {
    const requestOptions = {
      method: "GET",
      headers: authHeader(),
    };

    const url = `${import.meta.env.VITE_API_URL}/PlantWarehouse/GetPlantWarehouseDDL?unitId=${unitId}`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
