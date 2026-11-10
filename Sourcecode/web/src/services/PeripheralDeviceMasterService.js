import { authHeader, handleResponse } from "../helper/auth-header";

// Frontend XSS / Malicious Input Validation
const xssPattern =
  /<script\b[^>]*>[\s\S]*?<\/script\s*>|javascript\s*:|on\w+\s*=/i;

const validateInput = (obj) => {
  const checkValue = (value) => {
    if (typeof value === "string") {
      return !xssPattern.test(value);
    }

    if (Array.isArray(value)) {
      return value.every(checkValue);
    }

    if (value && typeof value === "object") {
      return Object.values(value).every(checkValue);
    }

    return true;
  };

  return checkValue(obj);
};

export default class PeripheralDeviceMasterService {
  async GetPeripheralDevices(obj) {
    // Frontend XSS validation
    if (!validateInput(obj)) {
      throw new Error(
        "Invalid input. Script or malicious content is not allowed."
      );
    }

    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/PeripheralDevices/GetPeripheralDevices`;

    const response = await fetch(url, requestOptions);

    const data = await handleResponse(response, {
      url,
      ...requestOptions,
    });

    return data;
  }

  async CreateUpdatePeripheralDevice(obj) {
    // Frontend XSS validation
    if (!validateInput(obj)) {
      throw new Error(
        "Invalid input. Script or malicious content is not allowed."
      );
    }

    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/PeripheralDevices/CreateUpdatePeripheralDevices`;

    const response = await fetch(url, requestOptions);

    const data = await handleResponse(response, {
      url,
      ...requestOptions,
    });

    return data;
  }
}