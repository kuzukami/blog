import React from 'react';
import Select from "react-select";
import { useForm, useController, UseControllerProps } from "react-hook-form";
import {List, ListItem} from '@mui/material';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

type FormValues = {
  FirstName: string;
};

function Input(props: UseControllerProps<FormValues>) {
  const { field, fieldState } = useController(props);

  return (
    <div>
      <input {...field} placeholder={props.name} />
      <p>{fieldState.isTouched && "Touched"}</p>
      <p>{fieldState.isDirty && "Dirty"}</p>
      <p>{fieldState.invalid ? "invalid" : "valid"}</p>
    </div>
  );
}

const Home7 = () => {
  const { handleSubmit, control } = useForm<FormValues>({
    defaultValues: {
      FirstName: ""
    },
    mode: "onChange"
  });
  const onSubmit = (data: FormValues) => {
    console.log(data);
  }
  
  return (
    <ResponsiveDrawer>
      <div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <List>
            <ListItem>
              <Input control={control} name="FirstName" rules={{ required: true }} />
            </ListItem><ListItem>
              <input type="submit" />
            </ListItem>
          </List>
        </form>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home7;