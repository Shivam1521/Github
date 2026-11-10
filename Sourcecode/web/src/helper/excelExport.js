import * as XLSX from "xlsx";

export const exportToExcel = (data, fileName = "export.xlsx") => {
  if (!data || data.length === 0) {
    alert("No data to export");
    return;
  }

  try {
    // Create a new workbook
    const workbook = XLSX.utils.book_new();

    // Convert the data to a sheet
    const worksheet = XLSX.utils.json_to_sheet(data);

    // Add the worksheet to the workbook
    XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

    // Style the header row (optional enhancement)
    const range = XLSX.utils.decode_range(worksheet["!ref"]);
    for (let C = range.s.c; C <= range.e.c; ++C) {
      const address = XLSX.utils.encode_col(C) + "1";
      if (!worksheet[address]) continue;
      worksheet[address].s = {
        font: { bold: true, color: { rgb: "FFFFFF" } },
        fill: { fgColor: { rgb: "366092" } },
        alignment: { horizontal: "center", vertical: "center" },
      };
    }

    // Auto-adjust column widths
    const colWidths = [];
    const range2 = XLSX.utils.decode_range(worksheet["!ref"]);
    for (let C = range2.s.c; C <= range2.e.c; ++C) {
      let maxLength = 0;
      for (let R = range2.s.r; R <= range2.e.r; ++R) {
        const cell = worksheet[XLSX.utils.encode_cell({ r: R, c: C })];
        if (cell && cell.v) {
          const cellLength = String(cell.v).length;
          if (cellLength > maxLength) {
            maxLength = cellLength;
          }
        }
      }
      colWidths.push({ wch: Math.max(maxLength + 2, 12) });
    }
    worksheet["!cols"] = colWidths;

    // Generate the Excel file and download it
    XLSX.writeFile(workbook, fileName);
  } catch (error) {
    console.error("Error exporting to Excel:", error);
    alert("Failed to export data to Excel");
  }
};

export const exportListDataToExcel = (data, fileName = "list_data.xlsx") => {
  // Transform the data to make it more readable in Excel
  const transformedData = data.map((item) => ({
    "List Name": item.listName || "",
    "List Detail Name": item.listDetailName || "",
    "Active": item.isActive ? "Yes" : "No",
    "Created Date": item.createDate || "",
    "Modified Date": item.modifyDate || "",
  }));

  exportToExcel(transformedData, fileName);
};

export const exportItemDataToExcel = (data, fileName = "item_data.xlsx") => {
  // Transform the data to make it more readable in Excel
  const transformedData = data.map((item) => ({
    "ID": item.id || "",
    "Item Code": item.itemCode || "",
    "Description": item.description || "",
    "Brand": item.brand || "",
    "Mfg Location": item.mfgLocation || "",
    "Class": item.class || "",
    "Master Carton Packing": item.masterCartonPackingSize || "",
    "Outer Box Packing": item.outerBoxPackingSize || "",
  }));

  exportToExcel(transformedData, fileName);
};

export const exportEmployeeDataToExcel = (data, fileName = "employee_data.xlsx") => {
  // Transform the data to make it more readable in Excel
  const transformedData = data.map((item) => ({
    "Emp Code": item.userCode || "",
    "Employee Name": item.userName || "",
    "Mobile Number": item.mobileNo || "",
    "Email": item.email || "",
    "Role": item.roleName || "",
    "Unit": item.unitName || "",
    "WH Zone": item.whZoneName || "",
    "Employee Type": item.userTypeName || "",
    "Active": item.isActive ? "Yes" : "No",
  }));

  exportToExcel(transformedData, fileName);
};

export const exportPeripheralDeviceDataToExcel = (data, fileName = "peripheral_device_data.xlsx") => {
  // Transform the data to make it more readable in Excel
  const transformedData = data.map((item) => ({
    "Line No": item.lineNo || "",
    "Device Type": item.deviceType || "",
    "IP": item.ip || "",
    "Port": item.port || "",
    "Brand": item.brand || "",
    "Active": item.isActive ? "Yes" : "No",
  }));

  exportToExcel(transformedData, fileName);
};
