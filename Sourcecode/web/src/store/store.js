import { configureStore } from "@reduxjs/toolkit";
import employeeMasterReducer from "./employeeMasterSlice";
import listReducer from "./commonSlice";
import roleReducer from "./roleMasterSlice";
import unitMasterReducer from "./unitMasterSlice";
import itemMasterReducer from "./itemMasterSlice";
import plantWarehouseReducer from "./warehouseMasterSlice";
import serialUploadReducer from "./printingSlice";
import printingRptReducer from "./printingReportSlice";
import listMasterReducer from "./listMasterSlice";
import peripheralDeviceReducer from "./peripheralDeviceMasterSlice";
import scratchGenReducer from "./scratchGenSlice";
import dashboardReducer from "./dashboardSlice";
import unitReducer from "./unitSlice";
import pickSlipStatusReducer from "./pickSlipStatusSlice";
import pickedSlipRptReducer from "./pickedSlipRptSlice";

export const store = configureStore({
  reducer: {
    employeeMaster: employeeMasterReducer,
    lists: listReducer,
    roleMaster: roleReducer,
    unitMaster: unitMasterReducer,
    itemMaster: itemMasterReducer,
    plantWarehouseMaster: plantWarehouseReducer,
    serialUpload: serialUploadReducer,
    printingRpt: printingRptReducer,
    listMaster: listMasterReducer,
    peripheralDeviceMaster: peripheralDeviceReducer,
    scratchGeneration: scratchGenReducer,
    dashboard: dashboardReducer,
    unit: unitReducer,
    pickSlipStatus: pickSlipStatusReducer,
    pickedSlipRpt: pickedSlipRptReducer,
  },
});
